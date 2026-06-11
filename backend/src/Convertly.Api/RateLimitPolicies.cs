namespace Convertly.Api;

internal static class RateLimitPolicies
{
    public const string AuthSensitive = "auth-sensitive";
    public const string AccountSensitive = "account-sensitive";
    public const string ConversionCreate = "conversion-create";
    public const string ConversionDownload = "conversion-download";
}
