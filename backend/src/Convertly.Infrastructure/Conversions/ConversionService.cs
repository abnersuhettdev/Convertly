using Convertly.Application.Auth;
using Convertly.Application.Common;
using Convertly.Application.Conversions;
using Convertly.Application.Conversions.Dtos;
using Convertly.Application.Files;
using Convertly.Application.Files.Dtos;
using Convertly.Application.Subscriptions;
using Convertly.Domain.Constants;
using Convertly.Domain.Entities;
using Convertly.Domain.Enums;
using Convertly.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Convertly.Infrastructure.Conversions;

public sealed class ConversionService(
    ConvertlyDbContext dbContext,
    ICurrentUserService currentUserService,
    IMonthlyUsageService monthlyUsageService,
    IFileStorageService fileStorageService,
    IConversionJobQueue conversionJobQueue,
    IDateTimeProvider dateTimeProvider) : IConversionService
{
    private const string DocxMimeType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
    private const string PdfContentType = "application/pdf";
    private const int DefaultPage = 1;
    private const int DefaultPageSize = 10;
    private const int MaxPageSize = 50;

    public async Task<ApiResponse<CreateConversionResponse>> CreateConversionAsync(
        CreateConversionRequest request,
        CancellationToken cancellationToken)
    {
        if (!currentUserService.IsAuthenticated || currentUserService.UserId is null)
        {
            return ApiResponse<CreateConversionResponse>.Fail("Unauthorized", "Authenticated user was not found");
        }

        var userId = currentUserService.UserId.Value;
        var validationErrors = await ValidateRequestAsync(userId, request, cancellationToken);
        if (validationErrors.Count > 0)
        {
            return ApiResponse<CreateConversionResponse>.Fail("Validation failed", validationErrors.ToArray());
        }

        FileStorageResult? storageResult = null;
        var usageReserved = false;
        Guid? conversionId = null;
        Guid? sourceFileId = null;

        try
        {
            var reservation = await monthlyUsageService.ReserveConversionAsync(userId, cancellationToken);
            if (!reservation.Success)
            {
                return ApiResponse<CreateConversionResponse>.Fail(
                    reservation.Message,
                    reservation.Errors.ToArray());
            }

            usageReserved = true;

            conversionId = Guid.NewGuid();
            sourceFileId = Guid.NewGuid();
            var now = dateTimeProvider.UtcNow;

            storageResult = await fileStorageService.SaveOriginalAsync(
                request.File,
                userId,
                conversionId.Value,
                sourceFileId.Value,
                request.FileName,
                request.ContentType,
                cancellationToken);

            await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

            var fileAsset = new FileAsset
            {
                Id = sourceFileId.Value,
                UserId = userId,
                OriginalFileName = request.FileName,
                StoredFileName = storageResult.StoredFileName,
                StoragePath = storageResult.StoragePath,
                BucketName = storageResult.BucketName,
                Extension = SupportedFormats.Docx,
                MimeType = request.ContentType,
                SizeBytes = request.SizeBytes,
                Kind = FileAssetKind.Original,
                CreatedAt = now
            };

            var conversionJob = new ConversionJob
            {
                Id = conversionId.Value,
                UserId = userId,
                SourceFileId = sourceFileId.Value,
                OutputFileId = null,
                SourceFormat = SupportedFormats.Docx,
                TargetFormat = SupportedFormats.Pdf,
                Status = ConversionStatus.Pending,
                UsageReserved = true,
                CreatedAt = now
            };

            dbContext.FileAssets.Add(fileAsset);
            dbContext.ConversionJobs.Add(conversionJob);

            await dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            conversionJobQueue.EnqueueConversionJob(conversionId.Value);

            return ApiResponse<CreateConversionResponse>.Ok(
                new CreateConversionResponse(conversionId.Value, ConversionStatus.Pending.ToString()),
                "Conversion job created");
        }
        catch
        {
            if (conversionId is not null && sourceFileId is not null)
            {
                await TryRemoveCreatedDatabaseRowsAsync(conversionId.Value, sourceFileId.Value, cancellationToken);
            }

            if (storageResult is not null)
            {
                await TryDeleteStoredFileAsync(storageResult, cancellationToken);
            }

            if (usageReserved)
            {
                await monthlyUsageService.ReturnConversionAsync(userId, cancellationToken);
            }

            return ApiResponse<CreateConversionResponse>.Fail(
                "Conversion creation failed",
                "Could not create conversion job");
        }
    }

    public async Task<ApiResponse<ConversionListResponse>> GetConversionsAsync(
        int page,
        int pageSize,
        string? status,
        CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUserId(out var userId))
        {
            return ApiResponse<ConversionListResponse>.Fail("Unauthorized", "Authenticated user was not found");
        }

        if (!TryParseStatus(status, out var parsedStatus, out var statusError))
        {
            return ApiResponse<ConversionListResponse>.Fail("Validation failed", statusError);
        }

        var normalizedPage = page < DefaultPage ? DefaultPage : page;
        var normalizedPageSize = pageSize <= 0 ? DefaultPageSize : Math.Min(pageSize, MaxPageSize);

        var query = dbContext.ConversionJobs
            .AsNoTracking()
            .Include(job => job.SourceFile)
            .Include(job => job.OutputFile)
            .Where(job => job.UserId == userId);

        if (parsedStatus is not null)
        {
            query = query.Where(job => job.Status == parsedStatus.Value);
        }

        var totalItems = await query.CountAsync(cancellationToken);
        var totalPages = totalItems == 0
            ? 0
            : (int)Math.Ceiling(totalItems / (double)normalizedPageSize);

        var items = await query
            .OrderByDescending(job => job.CreatedAt)
            .Skip((normalizedPage - 1) * normalizedPageSize)
            .Take(normalizedPageSize)
            .Select(job => new ConversionListItemResponse(
                job.Id,
                job.SourceFile.OriginalFileName,
                job.SourceFormat,
                job.TargetFormat,
                job.Status.ToString(),
                job.CreatedAt,
                job.CompletedAt,
                IsDownloadAvailable(job, dateTimeProvider.UtcNow)))
            .ToListAsync(cancellationToken);

        return ApiResponse<ConversionListResponse>.Ok(
            new ConversionListResponse(items, normalizedPage, normalizedPageSize, totalItems, totalPages),
            "Conversions loaded");
    }

    public async Task<ApiResponse<ConversionDetailResponse>> GetConversionAsync(
        Guid conversionId,
        CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUserId(out var userId))
        {
            return ApiResponse<ConversionDetailResponse>.Fail("Unauthorized", "Authenticated user was not found");
        }

        var conversionJob = await dbContext.ConversionJobs
            .AsNoTracking()
            .Include(job => job.SourceFile)
            .Include(job => job.OutputFile)
            .SingleOrDefaultAsync(
                job => job.Id == conversionId && job.UserId == userId,
                cancellationToken);

        if (conversionJob is null)
        {
            return ApiResponse<ConversionDetailResponse>.Fail("Conversion not found", "Conversion was not found");
        }

        return ApiResponse<ConversionDetailResponse>.Ok(
            ToDetailResponse(conversionJob),
            "Conversion loaded");
    }

    public async Task<ApiResponse<ConversionDownloadResponse>> DownloadConversionAsync(
        Guid conversionId,
        CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUserId(out var userId))
        {
            return ApiResponse<ConversionDownloadResponse>.Fail("Unauthorized", "Authenticated user was not found");
        }

        var conversionJob = await dbContext.ConversionJobs
            .AsNoTracking()
            .Include(job => job.SourceFile)
            .Include(job => job.OutputFile)
            .SingleOrDefaultAsync(
                job => job.Id == conversionId && job.UserId == userId,
                cancellationToken);

        if (conversionJob is null)
        {
            return ApiResponse<ConversionDownloadResponse>.Fail("Conversion not found", "Conversion was not found");
        }

        if (!IsDownloadAvailable(conversionJob, dateTimeProvider.UtcNow) || conversionJob.OutputFile is null)
        {
            return ApiResponse<ConversionDownloadResponse>.Fail(
                "Download unavailable",
                "Converted file is not available for download");
        }

        try
        {
            var stream = await fileStorageService.GetAsync(
                conversionJob.OutputFile.BucketName,
                conversionJob.OutputFile.StoragePath,
                cancellationToken);

            return ApiResponse<ConversionDownloadResponse>.Ok(
                new ConversionDownloadResponse(
                    stream,
                    BuildDownloadFileName(conversionJob.SourceFile.OriginalFileName),
                    PdfContentType),
                "Download ready");
        }
        catch
        {
            return ApiResponse<ConversionDownloadResponse>.Fail(
                "Download failed",
                "Converted file was not found");
        }
    }

    private async Task<List<string>> ValidateRequestAsync(
        Guid userId,
        CreateConversionRequest request,
        CancellationToken cancellationToken)
    {
        var errors = new List<string>();

        if (request.File is null)
        {
            errors.Add("File is required");
            return errors;
        }

        if (request.SizeBytes <= 0)
        {
            errors.Add("File must not be empty");
        }

        if (!Path.GetExtension(request.FileName).Equals(".docx", StringComparison.OrdinalIgnoreCase))
        {
            errors.Add("File extension is not supported");
        }

        if (!request.ContentType.Equals(DocxMimeType, StringComparison.OrdinalIgnoreCase))
        {
            errors.Add("File MIME type is not supported");
        }

        if (!request.TargetFormat.Equals(SupportedFormats.Pdf, StringComparison.OrdinalIgnoreCase))
        {
            errors.Add("Target format is not supported");
        }

        var activeSubscription = await dbContext.UserSubscriptions
            .AsNoTracking()
            .Include(subscription => subscription.Plan)
            .Where(subscription => subscription.UserId == userId && subscription.Status == SubscriptionStatus.Active)
            .OrderByDescending(subscription => subscription.StartedAt)
            .ThenByDescending(subscription => subscription.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

        if (activeSubscription is null)
        {
            errors.Add("Active subscription was not found");
            return errors;
        }

        if (!activeSubscription.Plan.IsActive)
        {
            errors.Add("Current plan is inactive");
        }

        var maxFileSizeBytes = activeSubscription.Plan.MaxFileSizeMb * 1024L * 1024L;
        if (request.SizeBytes > maxFileSizeBytes)
        {
            errors.Add("File exceeds current plan size limit");
        }

        return errors;
    }

    private bool TryGetCurrentUserId(out Guid userId)
    {
        if (currentUserService.IsAuthenticated && currentUserService.UserId is not null)
        {
            userId = currentUserService.UserId.Value;
            return true;
        }

        userId = Guid.Empty;
        return false;
    }

    private static bool TryParseStatus(
        string? status,
        out ConversionStatus? parsedStatus,
        out string error)
    {
        parsedStatus = null;
        error = string.Empty;

        if (string.IsNullOrWhiteSpace(status))
        {
            return true;
        }

        if (Enum.TryParse<ConversionStatus>(status, ignoreCase: true, out var value)
            && Enum.IsDefined(value))
        {
            parsedStatus = value;
            return true;
        }

        error = "Conversion status is invalid";
        return false;
    }

    private ConversionDetailResponse ToDetailResponse(ConversionJob conversionJob)
    {
        return new ConversionDetailResponse(
            conversionJob.Id,
            conversionJob.SourceFile.OriginalFileName,
            conversionJob.SourceFormat,
            conversionJob.TargetFormat,
            conversionJob.Status.ToString(),
            conversionJob.ErrorMessage,
            conversionJob.CreatedAt,
            conversionJob.StartedAt,
            conversionJob.CompletedAt,
            conversionJob.ExpiresAt,
            IsDownloadAvailable(conversionJob, dateTimeProvider.UtcNow));
    }

    private static bool IsDownloadAvailable(ConversionJob conversionJob, DateTime now)
    {
        return conversionJob.Status == ConversionStatus.Completed
            && conversionJob.OutputFileId is not null
            && conversionJob.ExpiresAt is not null
            && conversionJob.ExpiresAt > now
            && (conversionJob.OutputFile?.ExpiresAt is null || conversionJob.OutputFile.ExpiresAt > now);
    }

    private static string BuildDownloadFileName(string sourceFileName)
    {
        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(sourceFileName);
        return string.IsNullOrWhiteSpace(fileNameWithoutExtension)
            ? "converted.pdf"
            : $"{fileNameWithoutExtension}.pdf";
    }

    private async Task TryDeleteStoredFileAsync(FileStorageResult storageResult, CancellationToken cancellationToken)
    {
        try
        {
            await fileStorageService.DeleteAsync(storageResult.BucketName, storageResult.StoragePath, cancellationToken);
        }
        catch
        {
            // Best-effort cleanup. The caller receives a controlled failure response.
        }
    }

    private async Task TryRemoveCreatedDatabaseRowsAsync(
        Guid conversionJobId,
        Guid sourceFileId,
        CancellationToken cancellationToken)
    {
        try
        {
            var conversionJob = await dbContext.ConversionJobs.SingleOrDefaultAsync(
                job => job.Id == conversionJobId,
                cancellationToken);
            if (conversionJob is not null)
            {
                dbContext.ConversionJobs.Remove(conversionJob);
            }

            var fileAsset = await dbContext.FileAssets.SingleOrDefaultAsync(
                file => file.Id == sourceFileId,
                cancellationToken);
            if (fileAsset is not null)
            {
                dbContext.FileAssets.Remove(fileAsset);
            }

            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch
        {
            // Best-effort cleanup. The caller receives a controlled failure response.
        }
    }
}
