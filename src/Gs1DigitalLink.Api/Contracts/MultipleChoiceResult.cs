namespace Gs1DigitalLink.Api.Contracts;

public sealed record MultipleChoiceResult
{
    public required IEnumerable<LinkDefinition> Links { get; init; }
}