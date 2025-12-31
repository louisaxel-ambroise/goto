using Gs1DigitalLink.Core.Conversion;
using Gs1DigitalLink.Core.Conversion.Utils;
using Gs1DigitalLink.Core.Registration;
using Tavis.UriTemplates;

namespace Gs1DigitalLink.Core.Resolution;

public interface IDigitalLinkResolver
{
    IEnumerable<Link> GetCandidates(DigitalLink digitalLink, string? linkType);
    IEnumerable<Link> GetLinkset(DigitalLink digitalLink);
}

internal sealed class DigitalLinkResolver(IPrefixRegistry prefixRegistry, ILanguageContext languageContext) : IDigitalLinkResolver
{
    public IEnumerable<Link> GetLinkset(DigitalLink digitalLink)
    {
        var links = LoadCandidates(digitalLink);

        return FormatUriTemplates(links, digitalLink);
    }

    public IEnumerable<Link> GetCandidates(DigitalLink digitalLink, string? linkType)
    {
        var matchingLinks = LoadCandidates(digitalLink);
        var languages = languageContext.GetLanguages();

        if (linkType is not null)
        {
            matchingLinks = matchingLinks.Where(l => l.LinkType == linkType);
        }
        else
        {
            matchingLinks = matchingLinks.Where(l => l.LinkType == "gs1:defaultLink" || l.LinkType == "gs1:defaultLinkMulti");
        }

        var filteredLinks = FilterByLanguage(matchingLinks, languages);

        matchingLinks = filteredLinks.Any()
            ? filteredLinks
            : matchingLinks;

        return FormatUriTemplates(matchingLinks, digitalLink);
    }

    private IEnumerable<Link> LoadCandidates(DigitalLink digitalLink)
    {
        var prefixes = GetPrefixes(digitalLink);

        return prefixRegistry.Resolve(digitalLink.CompanyPrefix, prefixes);
    }

    private static IEnumerable<Link> FilterByLanguage(IEnumerable<Link> matchingLinks, IEnumerable<LanguagePreference> languages)
    {
        if (!languages.Any()) return matchingLinks;

        foreach (var language in languages.OrderByDescending(l => l.Quality))
        {
            var sortedLinks = matchingLinks.Where(link => LanguageMatches(link, language));

            if (sortedLinks.Any())
            {
                return sortedLinks.Any(link => RegionMatches(link, language))
                    ? sortedLinks.Where(link => RegionMatches(link, language))
                    : sortedLinks;
            }
        }

        return matchingLinks.Where(link => link.Language is null);
    }

    private static bool RegionMatches(Link link, LanguagePreference language)
    {
        if (link.Language is null) return false;

        var parts = link.Language.Split('-');

        return parts.Length > 1 && language.Region is not null && parts[1].Equals(language.Region, StringComparison.OrdinalIgnoreCase);
    }

    private static bool LanguageMatches(Link link, LanguagePreference language)
    {
        if (link.Language is null) return false;

        return link.Language.Split('-')[0].Equals(language.Language, StringComparison.OrdinalIgnoreCase);
    }

    private static List<string> GetPrefixes(DigitalLink digitalLink)
    {
        var prefixes = new List<string>();
        var key = digitalLink.AIs.Single(ai => ai.Key.Type is AIType.PrimaryKey);

        prefixes.Add(string.Join("/", key.Code, key.Value));

        foreach (var qualifier in digitalLink.AIs.Where(ai => ai.Key.Type is AIType.Qualifier))
        {
            prefixes.Add(string.Join("/", prefixes.Last(), qualifier.Code, qualifier.Value));
        }

        return prefixes;
    }

    private static IEnumerable<Link> FormatUriTemplates(IEnumerable<Link> linkset, DigitalLink digitalLink)
    {
        var parameters = GetDigitalLinkParameters(digitalLink);

        foreach (var link in linkset)
        {
            var template = new UriTemplate(link.RedirectUrl, false, false);

            template.AddParameters(parameters);

            yield return link with { RedirectUrl = template.Resolve() };
        }
    }

    private static Dictionary<string, object> GetDigitalLinkParameters(DigitalLink digitalLink)
    {
        var parameters = new Dictionary<string, object>();

        foreach (var ai in digitalLink.AIs)
        {
            parameters[ai.Key.Code] = ai.Value;

            if (!string.IsNullOrEmpty(ai.Key.ShortCode))
            {
                parameters[ai.Key.ShortCode] = ai.Value;
            }
        }

        return parameters;
    }
}
