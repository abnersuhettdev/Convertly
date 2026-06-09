using System.Diagnostics;
using Convertly.Application.Conversions;
using Convertly.Application.Conversions.Dtos;
using Convertly.Domain.Constants;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Convertly.Infrastructure.Conversions;

public sealed class DocxToPdfConverter(
    IOptions<ConversionOptions> options,
    ILogger<DocxToPdfConverter> logger) : IFileConverter
{
    private const string PdfContentType = "application/pdf";
    private readonly ConversionOptions _options = options.Value;

    public bool CanConvert(string sourceFormat, string targetFormat)
    {
        return sourceFormat.Equals(SupportedFormats.Docx, StringComparison.OrdinalIgnoreCase)
            && targetFormat.Equals(SupportedFormats.Pdf, StringComparison.OrdinalIgnoreCase);
    }

    public async Task<ConversionResult> ConvertAsync(
        ConversionRequest request,
        CancellationToken cancellationToken)
    {
        if (!CanConvert(request.SourceFormat, request.TargetFormat))
        {
            throw new InvalidOperationException("Unsupported conversion request.");
        }

        Directory.CreateDirectory(request.WorkingDirectory);

        var inputPath = Path.Combine(request.WorkingDirectory, "input.docx");
        var outputPath = Path.Combine(request.WorkingDirectory, "input.pdf");

        await using (var fileStream = File.Create(inputPath))
        {
            if (request.SourceFile.CanSeek)
            {
                request.SourceFile.Position = 0;
            }

            await request.SourceFile.CopyToAsync(fileStream, cancellationToken);
        }

        var startInfo = new ProcessStartInfo
        {
            FileName = string.IsNullOrWhiteSpace(_options.LibreOfficePath)
                ? "libreoffice"
                : _options.LibreOfficePath,
            RedirectStandardError = true,
            RedirectStandardOutput = true,
            UseShellExecute = false
        };
        startInfo.ArgumentList.Add("--headless");
        startInfo.ArgumentList.Add("--convert-to");
        startInfo.ArgumentList.Add("pdf");
        startInfo.ArgumentList.Add("--outdir");
        startInfo.ArgumentList.Add(request.WorkingDirectory);
        startInfo.ArgumentList.Add(inputPath);

        using var process = Process.Start(startInfo)
            ?? throw new InvalidOperationException("LibreOffice process could not be started.");

        var stdoutTask = process.StandardOutput.ReadToEndAsync(cancellationToken);
        var stderrTask = process.StandardError.ReadToEndAsync(cancellationToken);
        var timeout = TimeSpan.FromSeconds(Math.Max(1, _options.LibreOfficeTimeoutSeconds));
        var completedTask = await Task.WhenAny(process.WaitForExitAsync(cancellationToken), Task.Delay(timeout, cancellationToken));

        if (!completedTask.IsCompletedSuccessfully || !process.HasExited)
        {
            TryKillProcess(process);
            throw new InvalidOperationException("LibreOffice conversion timed out.");
        }

        var stdout = await stdoutTask;
        var stderr = await stderrTask;

        if (process.ExitCode != 0)
        {
            logger.LogError(
                "LibreOffice failed for conversion job {ConversionJobId}. ExitCode: {ExitCode}. Stdout: {Stdout}. Stderr: {Stderr}",
                request.ConversionJobId,
                process.ExitCode,
                stdout,
                stderr);
            throw new InvalidOperationException("LibreOffice conversion failed.");
        }

        if (!File.Exists(outputPath))
        {
            logger.LogError(
                "LibreOffice finished without generating PDF for conversion job {ConversionJobId}. Stdout: {Stdout}. Stderr: {Stderr}",
                request.ConversionJobId,
                stdout,
                stderr);
            throw new InvalidOperationException("Converted PDF was not generated.");
        }

        var outputFileName = $"{Path.GetFileNameWithoutExtension(request.SourceFileName)}.pdf";
        var sizeBytes = new FileInfo(outputPath).Length;

        return new ConversionResult(outputPath, outputFileName, PdfContentType, sizeBytes);
    }

    private static void TryKillProcess(Process process)
    {
        try
        {
            process.Kill(entireProcessTree: true);
        }
        catch
        {
            // Best-effort cleanup. The processor will mark the job as failed.
        }
    }
}
