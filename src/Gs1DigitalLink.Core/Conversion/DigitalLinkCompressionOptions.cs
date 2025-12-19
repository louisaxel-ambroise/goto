namespace Gs1DigitalLink.Core.Conversion;

public sealed record DigitalLinkCompressionOptions
{
    public DLCompressionType CompressionType { get; set; }
    public bool CompressQueryString { get; set; }
}

public enum DLCompressionType
{
    Full,
    Partial
}