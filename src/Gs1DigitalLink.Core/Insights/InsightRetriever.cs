using Gs1DigitalLink.Core.Conversion;

namespace Gs1DigitalLink.Core.Insights;

public interface IInsightRetriever
{
    IEnumerable<ScanInsight> ListInsights(DigitalLink digitalLink, ListInsightsOptions options);
}

internal sealed class InsightRetriever(IInsightSink sink) : IInsightRetriever
{
    public IEnumerable<ScanInsight> ListInsights(DigitalLink digitalLink, ListInsightsOptions options)
    {
        return sink.ListInsights(digitalLink.CompanyPrefix, digitalLink.ToString(false), options);
    }
}

public sealed record ListInsightsOptions
{
    public int Days { get; set; }
}