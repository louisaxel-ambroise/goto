using System.ComponentModel.DataAnnotations;

namespace Gs1DigitalLink.Api.Contracts;

public sealed record LinkDefinition
{
    [Required]
    public required string Href { get; init; }
    [Required]
    public required string Title { get; init; }
    public required string[] Hreflang { get; init; }
}
