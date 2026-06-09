namespace Convertly.Infrastructure.Conversions;

public sealed class ConversionOptions
{
    public string LibreOfficePath { get; set; } = "libreoffice";
    public int LibreOfficeTimeoutSeconds { get; set; } = 120;
}
