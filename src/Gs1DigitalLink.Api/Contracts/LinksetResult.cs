namespace Gs1DigitalLink.Api.Contracts;

public sealed record LinksetResult
{
    public required IDictionary<string, IEnumerable<LinkDefinition>> Linkset { get; init; }
}
