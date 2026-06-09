namespace Convertly.Infrastructure.Storage;

public sealed class SupabaseStorageOptions
{
    public string Url { get; set; } = string.Empty;
    public string ServiceRoleKey { get; set; } = string.Empty;
    public string OriginalsBucket { get; set; } = string.Empty;
    public string ConvertedBucket { get; set; } = string.Empty;

    public void Validate()
    {
        var missingSettings = new List<string>();

        if (string.IsNullOrWhiteSpace(Url))
        {
            missingSettings.Add("Supabase__Url");
        }

        if (string.IsNullOrWhiteSpace(ServiceRoleKey))
        {
            missingSettings.Add("Supabase__ServiceRoleKey");
        }

        if (string.IsNullOrWhiteSpace(OriginalsBucket))
        {
            missingSettings.Add("Supabase__OriginalsBucket");
        }

        if (string.IsNullOrWhiteSpace(ConvertedBucket))
        {
            missingSettings.Add("Supabase__ConvertedBucket");
        }

        if (missingSettings.Count > 0)
        {
            throw new InvalidOperationException(
                $"Missing required Supabase Storage configuration: {string.Join(", ", missingSettings)}.");
        }

        if (!Uri.TryCreate(Url, UriKind.Absolute, out _))
        {
            throw new InvalidOperationException("Supabase__Url must be a valid absolute URL.");
        }
    }
}
