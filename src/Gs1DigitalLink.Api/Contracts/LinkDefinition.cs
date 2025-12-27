namespace Gs1DigitalLink.Api.Contracts;

public sealed record LinkDefinition
{
    public required string Href { get; init; }
    public required string Title { get; init; }
    public required string[] Hreflang { get; init; }
}
