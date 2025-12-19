using Gs1DigitalLink.Core.Conversion;
using Gs1DigitalLink.Core.Registration;
using Gs1DigitalLink.Core.Resolution;

namespace Gs1DigitalLink.Core.Insights;

internal sealed class InsightDigitalLinkResolver(IDigitalLinkResolver resolver, ILanguageContext languageContext, IInsightSink sink, TimeProvider timeProvider) : IDigitalLinkResolver
{
    public IEnumerable<Link> GetCandidates(DigitalLink digitalLink, string? linkType)
    {
        var result = resolver.GetCandidates(digitalLink, linkType).ToList();
        var insight = new ScanInsight
        {
            Timestamp = timeProvider.GetUtcNow(),
            LinkType = linkType,
            Languages = languageContext.GetLanguages().Select(language => language.ToString()),
            CandidateCount = result.Count
        };

        sink.Record(digitalLink.CompanyPrefix, digitalLink.ToString(false), insight);

        return result;
    }

    public IEnumerable<Link> GetLinkset(DigitalLink digitalLink)
    {
        return resolver.GetLinkset(digitalLink);
    }
}
