namespace Gs1DigitalLink.Api.Contracts;

public sealed record Linkset
{
    public required IEnumerable<LinkDefinition> Links { get; init; }
}
