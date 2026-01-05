using Dapper;
using Gs1DigitalLink.Core.Insights;
using System.Data;

namespace Gs1DigitalLink.Infrastructure;

internal sealed class SqliteInsightSink(IDbConnection connection, TimeProvider timeProvider) : IInsightSink
{
    public void Store(ScanInsight insight)
    {
        var record = new Insight
        {
            DigitalLink = insight.DigitalLink,
            Timestamp = insight.Timestamp.ToUnixTimeSeconds(),
            CandidateCount = insight.CandidateCount,
            Languages = string.Join(',', insight.Languages),
            LinkType = insight.LinkType
        };

        connection.Execute("INSERT INTO [insights](DigitalLink, Timestamp, CandidateCount, Languages, LinkType) VALUES(@DigitalLink, @Timestamp, @CandidateCount, @Languages, @LinkType)", record);
    }

    public IEnumerable<ScanInsight> ListInsights(string digitalLink, ListInsightsOptions options)
    {
        var minDate = timeProvider.GetUtcNow().Subtract(TimeSpan.FromDays(options.Days-1)).ToUnixTimeSeconds() - timeProvider.GetUtcNow().TimeOfDay.TotalSeconds;
        var insights = connection.Query<Insight>("SELECT DigitalLink, Timestamp, LinkType, Languages, CandidateCount FROM [insights] WHERE Timestamp >= @Timestamp AND DigitalLink = @DigitalLink", new { Timestamp = minDate, DigitalLink = digitalLink });

        return insights.Select(insight => new ScanInsight
            {
                DigitalLink = insight.DigitalLink,
                CandidateCount = insight.CandidateCount,
                Timestamp = DateTimeOffset.FromUnixTimeSeconds(insight.Timestamp),
                Languages = insight.Languages.Split(','),
                LinkType = insight.LinkType
            });
    }
}

public record Insight
{
    public required string DigitalLink { get; init; }
    public required long Timestamp { get; init; }
    public required string? LinkType { get; init; }
    public required string Languages { get; init; }
    public required int CandidateCount { get; init; }
}
