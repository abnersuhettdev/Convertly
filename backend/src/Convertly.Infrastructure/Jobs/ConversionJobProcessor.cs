using Convertly.Application.Common;
using Convertly.Application.Conversions;
using Convertly.Application.Conversions.Dtos;
using Convertly.Application.Files;
using Convertly.Application.Files.Dtos;
using Convertly.Domain.Constants;
using Convertly.Domain.Entities;
using Convertly.Domain.Enums;
using Convertly.Infrastructure.Persistence;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Convertly.Infrastructure.Jobs;

public sealed class ConversionJobProcessor(
    ConvertlyDbContext dbContext,
    IFileStorageService fileStorageService,
    IFileConverterResolver fileConverterResolver,
    IDateTimeProvider dateTimeProvider,
    ILogger<ConversionJobProcessor> logger) : IConversionJobProcessor
{
    private const string ConversionFailureMessage = "Conversion failed. Please try again later.";

    [AutomaticRetry(Attempts = 0)]
    public async Task ProcessAsync(Guid conversionJobId)
    {
        FileStorageResult? convertedStorageResult = null;
        var outputFileId = Guid.NewGuid();
        var workingDirectory = GetWorkingDirectory(conversionJobId);

        try
        {
            var conversionJob = await GetConversionJobAsync(conversionJobId);
            if (conversionJob is null)
            {
                logger.LogWarning("Conversion job {ConversionJobId} was not found.", conversionJobId);
                return;
            }

            if (!await TryPrepareForProcessingAsync(conversionJob))
            {
                return;
            }

            var converter = fileConverterResolver.Resolve(conversionJob.SourceFormat, conversionJob.TargetFormat);
            await using var sourceStream = await fileStorageService.GetAsync(
                conversionJob.SourceFile.BucketName,
                conversionJob.SourceFile.StoragePath,
                CancellationToken.None);

            var conversionResult = await converter.ConvertAsync(
                new ConversionRequest(
                    conversionJob.Id,
                    sourceStream,
                    conversionJob.SourceFile.OriginalFileName,
                    conversionJob.SourceFormat,
                    conversionJob.TargetFormat,
                    workingDirectory),
                CancellationToken.None);

            await using (var convertedStream = File.OpenRead(conversionResult.OutputFilePath))
            {
                convertedStorageResult = await fileStorageService.SaveConvertedAsync(
                    convertedStream,
                    conversionJob.UserId,
                    conversionJob.Id,
                    outputFileId,
                    conversionResult.FileName,
                    conversionResult.ContentType,
                    CancellationToken.None);
            }

            await CompleteConversionAsync(conversionJob.Id, outputFileId, convertedStorageResult, conversionResult);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Conversion job {ConversionJobId} failed.", conversionJobId);

            if (convertedStorageResult is not null)
            {
                await TryDeleteConvertedFileAsync(convertedStorageResult);
            }

            await MarkFailedAndReturnUsageAsync(conversionJobId);
        }
        finally
        {
            TryDeleteWorkingDirectory(workingDirectory);
        }
    }

    private async Task<ConversionJob?> GetConversionJobAsync(Guid conversionJobId)
    {
        return await dbContext.ConversionJobs
            .Include(job => job.SourceFile)
            .Include(job => job.User)
            .Include(job => job.OutputFile)
            .SingleOrDefaultAsync(job => job.Id == conversionJobId);
    }

    private async Task<bool> TryPrepareForProcessingAsync(ConversionJob conversionJob)
    {
        if (conversionJob.Status == ConversionStatus.Completed)
        {
            logger.LogInformation("Conversion job {ConversionJobId} is already completed.", conversionJob.Id);
            return false;
        }

        if (conversionJob.Status == ConversionStatus.Failed)
        {
            logger.LogInformation(
                "Conversion job {ConversionJobId} is failed and will not be reprocessed automatically.",
                conversionJob.Id);
            return false;
        }

        if (conversionJob.Status == ConversionStatus.Processing)
        {
            if (conversionJob.OutputFileId is not null)
            {
                logger.LogInformation(
                    "Conversion job {ConversionJobId} is already processing with an output file.",
                    conversionJob.Id);
                return false;
            }

            return true;
        }

        if (conversionJob.Status != ConversionStatus.Pending)
        {
            logger.LogInformation(
                "Conversion job {ConversionJobId} has status {Status}; no processing was applied.",
                conversionJob.Id,
                conversionJob.Status);
            return false;
        }

        conversionJob.Status = ConversionStatus.Processing;
        conversionJob.StartedAt ??= dateTimeProvider.UtcNow;
        conversionJob.ErrorMessage = null;
        await dbContext.SaveChangesAsync();

        return true;
    }

    private async Task CompleteConversionAsync(
        Guid conversionJobId,
        Guid outputFileId,
        FileStorageResult convertedStorageResult,
        ConversionResult conversionResult)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync();

        var conversionJob = await dbContext.ConversionJobs
            .Include(job => job.User)
            .SingleAsync(job => job.Id == conversionJobId);
        var retentionHours = await GetRetentionHoursAsync(conversionJob.UserId);
        var now = dateTimeProvider.UtcNow;
        var expiresAt = now.AddHours(retentionHours);

        var convertedFile = new FileAsset
        {
            Id = outputFileId,
            UserId = conversionJob.UserId,
            OriginalFileName = conversionResult.FileName,
            StoredFileName = convertedStorageResult.StoredFileName,
            StoragePath = convertedStorageResult.StoragePath,
            BucketName = convertedStorageResult.BucketName,
            Extension = SupportedFormats.Pdf,
            MimeType = conversionResult.ContentType,
            SizeBytes = conversionResult.SizeBytes,
            Kind = FileAssetKind.Converted,
            CreatedAt = now,
            ExpiresAt = expiresAt
        };

        dbContext.FileAssets.Add(convertedFile);
        conversionJob.OutputFileId = outputFileId;
        conversionJob.Status = ConversionStatus.Completed;
        conversionJob.CompletedAt = now;
        conversionJob.ExpiresAt = expiresAt;
        conversionJob.ErrorMessage = null;

        await dbContext.SaveChangesAsync();
        await transaction.CommitAsync();
    }

    private async Task<int> GetRetentionHoursAsync(Guid userId)
    {
        var activeSubscription = await dbContext.UserSubscriptions
            .Include(subscription => subscription.Plan)
            .Where(subscription => subscription.UserId == userId && subscription.Status == SubscriptionStatus.Active)
            .OrderByDescending(subscription => subscription.StartedAt)
            .ThenByDescending(subscription => subscription.CreatedAt)
            .FirstOrDefaultAsync();

        if (activeSubscription is null || !activeSubscription.Plan.IsActive)
        {
            throw new InvalidOperationException("Active subscription was not found.");
        }

        return activeSubscription.Plan.RetentionHours;
    }

    private async Task MarkFailedAndReturnUsageAsync(Guid conversionJobId)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync();

        var conversionJob = await dbContext.ConversionJobs
            .SingleOrDefaultAsync(job => job.Id == conversionJobId);
        if (conversionJob is null)
        {
            await transaction.CommitAsync();
            return;
        }

        conversionJob.Status = ConversionStatus.Failed;
        conversionJob.ErrorMessage = ConversionFailureMessage;
        conversionJob.OutputFileId = null;

        if (conversionJob.UsageReserved)
        {
            await ReturnReservedUsageAsync(conversionJob.UserId);
            conversionJob.UsageReserved = false;
        }

        await dbContext.SaveChangesAsync();
        await transaction.CommitAsync();
    }

    private async Task ReturnReservedUsageAsync(Guid userId)
    {
        var now = dateTimeProvider.UtcNow;
        var monthlyUsage = await dbContext.MonthlyUsages.SingleOrDefaultAsync(
            usage => usage.UserId == userId && usage.Year == now.Year && usage.Month == now.Month);

        if (monthlyUsage is null)
        {
            return;
        }

        monthlyUsage.ConversionsUsed = Math.Max(0, monthlyUsage.ConversionsUsed - 1);
        monthlyUsage.UpdatedAt = now;
    }

    private async Task TryDeleteConvertedFileAsync(FileStorageResult storageResult)
    {
        try
        {
            await fileStorageService.DeleteAsync(
                storageResult.BucketName,
                storageResult.StoragePath,
                CancellationToken.None);
        }
        catch (Exception exception)
        {
            logger.LogWarning(
                exception,
                "Could not delete converted file {StoragePath} after failed conversion.",
                storageResult.StoragePath);
        }
    }

    private static string GetWorkingDirectory(Guid conversionJobId)
    {
        return Path.Combine(Path.GetTempPath(), "convertly", conversionJobId.ToString("N"));
    }

    private void TryDeleteWorkingDirectory(string workingDirectory)
    {
        try
        {
            if (Directory.Exists(workingDirectory))
            {
                Directory.Delete(workingDirectory, recursive: true);
            }
        }
        catch (Exception exception)
        {
            logger.LogWarning(
                exception,
                "Could not delete temporary conversion directory {WorkingDirectory}.",
                workingDirectory);
        }
    }
}
