namespace Gs1DigitalLink.Core.Insights;

public sealed record ScanInsight
{
    public required DateTimeOffset Timestamp { get; init; }
    public string? LinkType { get; init; }
    public IEnumerable<string> Languages { get; init; } = [];
    public int CandidateCount { get; init; }
}
