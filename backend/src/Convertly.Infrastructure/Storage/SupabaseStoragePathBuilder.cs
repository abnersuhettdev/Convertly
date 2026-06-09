using Convertly.Domain.Constants;

namespace Convertly.Infrastructure.Storage;

public static class SupabaseStoragePathBuilder
{
    public static string BuildOriginalPath(Guid userId, Guid conversionId, Guid fileId)
    {
        return $"users/{userId}/originals/{conversionId}/{fileId}.{SupportedFormats.Docx}";
    }

    public static string BuildConvertedPath(Guid userId, Guid conversionId, Guid fileId)
    {
        return $"users/{userId}/converted/{conversionId}/{fileId}.{SupportedFormats.Pdf}";
    }
}
