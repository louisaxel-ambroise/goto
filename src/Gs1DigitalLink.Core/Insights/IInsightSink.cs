namespace Gs1DigitalLink.Core.Insights;

public interface IInsightSink
{
    void Record(string companyPrefix, string digitalLink, ScanInsight insight);
    IEnumerable<ScanInsight> ListInsights(string companyPrefix, string digitalLink, ListInsightsOptions options);
}
