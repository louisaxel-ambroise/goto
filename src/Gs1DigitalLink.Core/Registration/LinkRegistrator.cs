using Gs1DigitalLink.Core.Conversion;

namespace Gs1DigitalLink.Core.Registration;

public interface ILinkRegistrator
{
    void RegisterLink(DigitalLink digitalLink, string redirectUrl, string title, string? language, IEnumerable<string> linkTypes);
    void DeleteLink(DigitalLink digitalLink, string? language, IEnumerable<string> linkTypes);
}

internal sealed class LinkRegistrator(IPrefixRegistry prefixRegistry) : ILinkRegistrator
{
    public void RegisterLink(DigitalLink digitalLink, string redirectUrl, string title, string? language, IEnumerable<string> linkTypes)
    {
        foreach (var linkType in linkTypes)
        {
            prefixRegistry.Register(digitalLink.CompanyPrefix, digitalLink.ToString(false), title, redirectUrl, language, linkType);
        }
    }

    public void DeleteLink(DigitalLink digitalLink, string? language, IEnumerable<string> linkTypes)
    {
        foreach (var linkType in linkTypes)
        {
            prefixRegistry.Unregister(digitalLink.CompanyPrefix, digitalLink.ToString(false), language, linkType);
        }
    }
}
