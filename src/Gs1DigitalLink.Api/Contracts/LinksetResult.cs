using System.Text.Json.Serialization;

namespace Gs1DigitalLink.Api.Contracts;

public sealed record LinksetResult
{
    public required IEnumerable<LinksetDefinition> Linkset { get; init; }
}

public class LinksetDefinition
{
    public required string Anchor { get; init; }

    [JsonExtensionData]
    public IDictionary<string, object> Links { get; init; }
}