using Convertly.Application.Conversions;
using Convertly.Application.Conversions.Dtos;
using Convertly.Application.Files;
using Convertly.Domain.Constants;
using Convertly.Domain.Entities;
using Convertly.Domain.Enums;
using Convertly.Infrastructure.Persistence;
using Convertly.Tests.Auth;
using Convertly.Tests.Conversions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Xunit;

namespace Convertly.Tests.Jobs;

public sealed class ConversionJobProcessorTests
{
    [Fact]
    public async Task ProcessAsync_ConvertsPendingJobToCompleted()
    {
        var fakeStorage = new FakeFileStorageService();
        var fakeConverter = new FakeFileConverter();
        using var factory = CreateFactory(fakeStorage, fakeConverter);
        var conversionJobId = await CreateConversionJobAsync(factory, ConversionStatus.Pending, conversionsUsed: 1);

        await ProcessAsync(factory, conversionJobId);

        await using var scope = factory.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ConvertlyDbContext>();
        var conversionJob = await dbContext.ConversionJobs.SingleAsync(job => job.Id == conversionJobId);

        Assert.Equal(ConversionStatus.Completed, conversionJob.Status);
        Assert.NotNull(conversionJob.StartedAt);
        Assert.NotNull(conversionJob.CompletedAt);
        Assert.NotNull(conversionJob.ExpiresAt);
        Assert.NotNull(conversionJob.OutputFileId);
        Assert.Null(conversionJob.ErrorMessage);
        Assert.True(conversionJob.UsageReserved);
    }

    [Fact]
    public async Task ProcessAsync_CreatesConvertedFileAssetAndOutputReference()
    {
        var fakeStorage = new FakeFileStorageService();
        using var factory = CreateFactory(fakeStorage, new FakeFileConverter());
        var conversionJobId = await CreateConversionJobAsync(factory, ConversionStatus.Pending, conversionsUsed: 1);

        await ProcessAsync(factory, conversionJobId);

        await using var scope = factory.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ConvertlyDbContext>();
        var conversionJob = await dbContext.ConversionJobs.SingleAsync(job => job.Id == conversionJobId);
        var convertedFile = await dbContext.FileAssets.SingleAsync(asset => asset.Kind == FileAssetKind.Converted);

        Assert.Equal(convertedFile.Id, conversionJob.OutputFileId);
        Assert.Equal(StorageBuckets.Converted, convertedFile.BucketName);
        Assert.Equal(SupportedFormats.Pdf, convertedFile.Extension);
        Assert.Equal("application/pdf", convertedFile.MimeType);
        Assert.EndsWith(".pdf", convertedFile.StoragePath);
    }

    [Fact]
    public async Task ProcessAsync_SetsExpirationFromCurrentPlanRetention()
    {
        using var factory = CreateFactory(new FakeFileStorageService(), new FakeFileConverter());
        var conversionJobId = await CreateConversionJobAsync(
            factory,
            ConversionStatus.Pending,
            conversionsUsed: 1,
            planId: PlanSeed.ProPlanId);

        await ProcessAsync(factory, conversionJobId);

        await using var scope = factory.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ConvertlyDbContext>();
        var conversionJob = await dbContext.ConversionJobs.SingleAsync(job => job.Id == conversionJobId);
        var convertedFile = await dbContext.FileAssets.SingleAsync(asset => asset.Kind == FileAssetKind.Converted);

        Assert.NotNull(conversionJob.CompletedAt);
        Assert.NotNull(conversionJob.ExpiresAt);
        Assert.Equal(conversionJob.ExpiresAt, convertedFile.ExpiresAt);
        Assert.InRange(
            (conversionJob.ExpiresAt.Value - conversionJob.CompletedAt.Value).TotalHours,
            167.99,
            168.01);
    }

    [Fact]
    public async Task ProcessAsync_DoesNotIncrementMonthlyUsageOnSuccess()
    {
        using var factory = CreateFactory(new FakeFileStorageService(), new FakeFileConverter());
        var conversionJobId = await CreateConversionJobAsync(factory, ConversionStatus.Pending, conversionsUsed: 1);

        await ProcessAsync(factory, conversionJobId);

        await using var scope = factory.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ConvertlyDbContext>();
        var usage = await dbContext.MonthlyUsages.SingleAsync();
        Assert.Equal(1, usage.ConversionsUsed);
    }

    [Fact]
    public async Task ProcessAsync_WhenConversionFails_MarksFailedAndReturnsUsage()
    {
        using var factory = CreateFactory(new FakeFileStorageService(), new FakeFileConverter { FailConversion = true });
        var conversionJobId = await CreateConversionJobAsync(factory, ConversionStatus.Pending, conversionsUsed: 1);

        await ProcessAsync(factory, conversionJobId);

        await using var scope = factory.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ConvertlyDbContext>();
        var conversionJob = await dbContext.ConversionJobs.SingleAsync(job => job.Id == conversionJobId);
        var usage = await dbContext.MonthlyUsages.SingleAsync();

        Assert.Equal(ConversionStatus.Failed, conversionJob.Status);
        Assert.Equal("Conversion failed. Please try again later.", conversionJob.ErrorMessage);
        Assert.False(conversionJob.UsageReserved);
        Assert.Null(conversionJob.OutputFileId);
        Assert.Equal(0, usage.ConversionsUsed);
        Assert.Empty(await dbContext.FileAssets.Where(asset => asset.Kind == FileAssetKind.Converted).ToListAsync());
    }

    [Fact]
    public async Task ProcessAsync_DoesNotReturnUsageTwiceOnRepeatedFailure()
    {
        using var factory = CreateFactory(new FakeFileStorageService(), new FakeFileConverter { FailConversion = true });
        var conversionJobId = await CreateConversionJobAsync(factory, ConversionStatus.Pending, conversionsUsed: 1);

        await ProcessAsync(factory, conversionJobId);
        await ProcessAsync(factory, conversionJobId);

        await using var scope = factory.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ConvertlyDbContext>();
        var usage = await dbContext.MonthlyUsages.SingleAsync();
        Assert.Equal(0, usage.ConversionsUsed);
    }

    [Fact]
    public async Task ProcessAsync_DoesNotReprocessCompletedJob()
    {
        var fakeConverter = new FakeFileConverter();
        using var factory = CreateFactory(new FakeFileStorageService(), fakeConverter);
        var conversionJobId = await CreateConversionJobAsync(factory, ConversionStatus.Completed, conversionsUsed: 1);

        await ProcessAsync(factory, conversionJobId);

        Assert.Equal(0, fakeConverter.ConversionCount);
    }

    [Fact]
    public async Task ProcessAsync_DoesNotReprocessFailedJob()
    {
        var fakeConverter = new FakeFileConverter();
        using var factory = CreateFactory(new FakeFileStorageService(), fakeConverter);
        var conversionJobId = await CreateConversionJobAsync(factory, ConversionStatus.Failed, conversionsUsed: 1);

        await ProcessAsync(factory, conversionJobId);

        Assert.Equal(0, fakeConverter.ConversionCount);
    }

    [Fact]
    public async Task ProcessAsync_IgnoresMissingJob()
    {
        using var factory = CreateFactory(new FakeFileStorageService(), new FakeFileConverter());

        await ProcessAsync(factory, Guid.NewGuid());

        await using var scope = factory.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ConvertlyDbContext>();
        Assert.Empty(await dbContext.ConversionJobs.ToListAsync());
    }

    [Fact]
    public async Task ProcessAsync_CleansTemporaryFiles()
    {
        var fakeConverter = new FakeFileConverter();
        using var factory = CreateFactory(new FakeFileStorageService(), fakeConverter);
        var conversionJobId = await CreateConversionJobAsync(factory, ConversionStatus.Pending, conversionsUsed: 1);

        await ProcessAsync(factory, conversionJobId);

        Assert.False(string.IsNullOrWhiteSpace(fakeConverter.LastWorkingDirectory));
        Assert.False(Directory.Exists(fakeConverter.LastWorkingDirectory));
    }

    private static ConvertlyApiFactory CreateFactory(
        FakeFileStorageService fakeStorage,
        FakeFileConverter fakeConverter)
    {
        return new ConvertlyApiFactory(services =>
        {
            services.RemoveAll<IFileStorageService>();
            services.RemoveAll<IFileConverter>();
            services.AddSingleton<IFileStorageService>(fakeStorage);
            services.AddSingleton<IFileConverter>(fakeConverter);
        });
    }

    private static async Task ProcessAsync(ConvertlyApiFactory factory, Guid conversionJobId)
    {
        await using var scope = factory.Services.CreateAsyncScope();
        var processor = scope.ServiceProvider.GetRequiredService<IConversionJobProcessor>();
        await processor.ProcessAsync(conversionJobId);
    }

    private static async Task<Guid> CreateConversionJobAsync(
        ConvertlyApiFactory factory,
        ConversionStatus status,
        int? conversionsUsed = null,
        Guid? planId = null)
    {
        await using var scope = factory.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ConvertlyDbContext>();

        var userId = Guid.NewGuid();
        var sourceFileId = Guid.NewGuid();
        var conversionJobId = Guid.NewGuid();
        var now = DateTime.UtcNow;

        dbContext.Users.Add(new User
        {
            Id = userId,
            Name = "Abner Suhett",
            Email = $"abner-{Guid.NewGuid():N}@example.com",
            PasswordHash = "hash",
            CreatedAt = now,
            IsActive = true
        });

        dbContext.UserSubscriptions.Add(new UserSubscription
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            PlanId = planId ?? PlanSeed.FreePlanId,
            Status = SubscriptionStatus.Active,
            StartedAt = now,
            CreatedAt = now
        });

        dbContext.FileAssets.Add(new FileAsset
        {
            Id = sourceFileId,
            UserId = userId,
            OriginalFileName = "document.docx",
            StoredFileName = $"{sourceFileId}.docx",
            StoragePath = $"users/{userId}/originals/{conversionJobId}/{sourceFileId}.docx",
            BucketName = StorageBuckets.Originals,
            Extension = SupportedFormats.Docx,
            MimeType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            SizeBytes = 100,
            Kind = FileAssetKind.Original,
            CreatedAt = now
        });

        dbContext.ConversionJobs.Add(new ConversionJob
        {
            Id = conversionJobId,
            UserId = userId,
            SourceFileId = sourceFileId,
            SourceFormat = SupportedFormats.Docx,
            TargetFormat = SupportedFormats.Pdf,
            Status = status,
            UsageReserved = true,
            CreatedAt = now
        });

        if (conversionsUsed is not null)
        {
            dbContext.MonthlyUsages.Add(new MonthlyUsage
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Year = now.Year,
                Month = now.Month,
                ConversionsUsed = conversionsUsed.Value,
                CreatedAt = now
            });
        }

        await dbContext.SaveChangesAsync();
        return conversionJobId;
    }
}

internal sealed class FakeFileConverter : IFileConverter
{
    public bool FailConversion { get; set; }
    public int ConversionCount { get; private set; }
    public string? LastWorkingDirectory { get; private set; }

    public bool CanConvert(string sourceFormat, string targetFormat)
    {
        return sourceFormat.Equals(SupportedFormats.Docx, StringComparison.OrdinalIgnoreCase)
            && targetFormat.Equals(SupportedFormats.Pdf, StringComparison.OrdinalIgnoreCase);
    }

    public async Task<ConversionResult> ConvertAsync(
        ConversionRequest request,
        CancellationToken cancellationToken)
    {
        ConversionCount += 1;
        LastWorkingDirectory = request.WorkingDirectory;

        if (FailConversion)
        {
            throw new InvalidOperationException("Fake conversion failed.");
        }

        Directory.CreateDirectory(request.WorkingDirectory);
        var outputFilePath = Path.Combine(request.WorkingDirectory, "input.pdf");
        await File.WriteAllBytesAsync(outputFilePath, [37, 80, 68, 70], cancellationToken);

        return new ConversionResult(outputFilePath, "document.pdf", "application/pdf", 4);
    }
}
