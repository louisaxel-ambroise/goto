using Gs1DigitalLink.Core.Conversion;
using Gs1DigitalLink.Core.Registration;
using Gs1DigitalLink.Core.Resolution;

namespace Gs1DigitalLink.Core.Insights;

internal sealed class InsightDigitalLinkResolver(IDigitalLinkResolver resolver, ILanguageContext languageContext, IInsightRecorder insightRecorder, TimeProvider timeProvider) : IDigitalLinkResolver
{
    public IEnumerable<Link> GetCandidates(DigitalLink digitalLink, DateTimeOffset applicability, string? linkType)
    {
        var result = resolver.GetCandidates(digitalLink, applicability, linkType).ToList();
        var insight = new ScanInsight
        {
            DigitalLink = digitalLink.ToString(false),
            Timestamp = timeProvider.GetUtcNow(),
            LinkType = linkType,
            Languages = languageContext.GetLanguages().Select(language => language.ToString()),
            CandidateCount = result.Count
        };

        insightRecorder.Record(insight);

        return result;
    }

    public IEnumerable<Link> GetLinkset(DigitalLink digitalLink, DateTimeOffset applicability)
    {
        return resolver.GetLinkset(digitalLink, applicability);
    }
}
