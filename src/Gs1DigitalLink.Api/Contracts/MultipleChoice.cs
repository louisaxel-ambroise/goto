namespace Gs1DigitalLink.Api.Contracts;

public sealed record MultipleChoice
{
    public required IEnumerable<LinkDefinition> Links { get; init; }
}