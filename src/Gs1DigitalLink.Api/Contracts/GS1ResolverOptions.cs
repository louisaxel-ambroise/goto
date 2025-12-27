namespace Gs1DigitalLink.Api.Contracts;

public sealed record GS1ResolverOptions
{
    public static readonly string Key = nameof(GS1ResolverOptions);

    public required string MainUrl { get; init; }
    public required string Name { get; init; }
    public required string[] SupportedPrimaryKeys { get; init; }
    public required string ContactName { get; init; }
}