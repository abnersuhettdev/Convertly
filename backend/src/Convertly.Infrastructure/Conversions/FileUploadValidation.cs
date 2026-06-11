using Convertly.Domain.Constants;

namespace Convertly.Infrastructure.Conversions;

internal static class FileUploadValidation
{
    public const string OfficialDocxMimeType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";

    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".docx"
    };

    private static readonly HashSet<string> BlockedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".exe",
        ".bat",
        ".cmd",
        ".sh",
        ".ps1",
        ".js",
        ".vbs",
        ".scr",
        ".msi",
        ".dll",
        ".jar",
        ".php",
        ".html",
        ".svg",
        ".py",
        ".rb"
    };

    private static readonly HashSet<string> AllowedDocxMimeTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        OfficialDocxMimeType,
        "application/zip",
        "application/octet-stream"
    };

    public static string GetSafeDisplayFileName(string fileName)
    {
        var safeFileName = Path.GetFileName(fileName);
        return string.IsNullOrWhiteSpace(safeFileName) ? "document.docx" : safeFileName;
    }

    public static FileUploadValidationResult Validate(
        string fileName,
        string contentType,
        long sizeBytes,
        string targetFormat,
        long maxFileSizeBytes)
    {
        var safeFileName = GetSafeDisplayFileName(fileName);
        var extension = Path.GetExtension(safeFileName);
        var errors = new List<FileUploadValidationError>();

        if (string.IsNullOrWhiteSpace(fileName) || string.IsNullOrWhiteSpace(extension))
        {
            errors.Add(FileUploadValidationError.UnsupportedExtension);
        }
        else if (BlockedExtensions.Contains(extension))
        {
            errors.Add(FileUploadValidationError.BlockedExtension);
        }
        else if (!AllowedExtensions.Contains(extension))
        {
            errors.Add(FileUploadValidationError.UnsupportedExtension);
        }

        if (sizeBytes <= 0)
        {
            errors.Add(FileUploadValidationError.EmptyFile);
        }

        if (sizeBytes > maxFileSizeBytes)
        {
            errors.Add(FileUploadValidationError.FileTooLarge);
        }

        if (!IsAllowedDocxMimeType(contentType))
        {
            errors.Add(FileUploadValidationError.UnsupportedMimeType);
        }

        if (!targetFormat.Equals(SupportedFormats.Pdf, StringComparison.OrdinalIgnoreCase))
        {
            errors.Add(FileUploadValidationError.UnsupportedTargetFormat);
        }

        return new FileUploadValidationResult(safeFileName, extension, errors);
    }

    private static bool IsAllowedDocxMimeType(string contentType)
    {
        if (string.IsNullOrWhiteSpace(contentType))
        {
            return true;
        }

        return AllowedDocxMimeTypes.Contains(contentType);
    }
}

internal sealed record FileUploadValidationResult(
    string SafeFileName,
    string Extension,
    IReadOnlyList<FileUploadValidationError> Errors);

internal enum FileUploadValidationError
{
    BlockedExtension,
    EmptyFile,
    FileTooLarge,
    UnsupportedExtension,
    UnsupportedMimeType,
    UnsupportedTargetFormat
}
