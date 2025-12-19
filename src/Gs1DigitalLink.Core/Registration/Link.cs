namespace Gs1DigitalLink.Core.Registration;

public sealed record Link
{
    public required string Prefix { get; init; }
    public required string RedirectUrl { get; init; }
    public required string Title { get; init; }
    public required string? Language { get; init; }
    public required string[] LinkTypes { get; init; }
}

public sealed record Linkset
{
    public required IEnumerable<Link> Links { get; set; }
}