namespace Gs1DigitalLink.Core.Insights;

public interface IInsightSink
{
    void Store(ScanInsight insight);
    IEnumerable<ScanInsight> ListInsights(string digitalLink, ListInsightsOptions options);
}
