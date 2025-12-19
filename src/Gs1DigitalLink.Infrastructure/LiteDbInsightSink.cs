using Gs1DigitalLink.Core.Insights;

namespace Gs1DigitalLink.Infrastructure;

internal sealed class LiteDbInsightSink(TimeProvider timeProvider) : IInsightSink
{
    public void Record(string companyPrefix, string digitalLink, ScanInsight insight)
    {
        using var connection = LiteDbConnectionProvider.Connect(companyPrefix);
        var collection = connection.GetCollection<Insight>();

        var record = new Insight
        {
            DigitalLink = digitalLink,
            Timestamp = insight.Timestamp,
            CandidateCount = insight.CandidateCount,
            Languages = insight.Languages,
            LinkType = insight.LinkType
        };

        collection.Insert(record);
    }

    public IEnumerable<ScanInsight> ListInsights(string companyPrefix, string digitalLink, ListInsightsOptions options)
    {
        using var connection = LiteDbConnectionProvider.Connect(companyPrefix);
        var collection = connection.GetCollection<Insight>();
        var minDate = timeProvider.GetUtcNow().Date.Subtract(TimeSpan.FromDays(options.Days-1));

        return collection.Query()
            .Where(insight => insight.DigitalLink == digitalLink)
            .Where(insight => insight.Timestamp >= minDate)
            .ToEnumerable()
            .Select(insight => new ScanInsight
            {
                CandidateCount = insight.CandidateCount,
                Timestamp = insight.Timestamp,
                Languages = insight.Languages,
                LinkType = insight.LinkType
            });
    }
}

public record Insight
{
    public int Id { get; set; }
    public required string DigitalLink { get; init; }
    public required DateTimeOffset Timestamp { get; init; }
    public required string? LinkType { get; init; }
    public required IEnumerable<string> Languages { get; init; }
    public required int CandidateCount { get; init; }
}
