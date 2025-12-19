using System.ComponentModel.DataAnnotations;

namespace Gs1DigitalLink.Api.Contracts;

public sealed record LinkDefinition
{
    [Required]
    public required string Prefix { get; init; }
    [Required]
    public required string RedirectUrl { get; init; }
    [Required]
    public required string Title { get; init; }
    [Required, MinLength(1)]
    public required string[] LinkTypes { get; init; }
    public required string? Language { get; init; }
}
