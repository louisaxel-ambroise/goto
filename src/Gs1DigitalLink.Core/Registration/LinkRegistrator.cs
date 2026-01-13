using Gs1DigitalLink.Core.Conversion;

namespace Gs1DigitalLink.Core.Registration;

public interface ILinkRegistrator
{
    void RegisterLink(DigitalLink digitalLink, string redirectUrl, string title, string? language, DateTimeRange applicability, IEnumerable<string> linkTypes);
    void DeleteLink(DigitalLink digitalLink, string? language, IEnumerable<string> linkTypes);
}

internal sealed class LinkRegistrator(IPrefixRegistry prefixRegistry) : ILinkRegistrator
{
    public void RegisterLink(DigitalLink digitalLink, string redirectUrl, string title, string? language, DateTimeRange applicability, IEnumerable<string> linkTypes)
    {
        prefixRegistry.Register(digitalLink.ToString(false), title, redirectUrl, language, applicability, linkTypes);
    }

    public void DeleteLink(DigitalLink digitalLink, string? language, IEnumerable<string> linkTypes)
    {
        prefixRegistry.Unregister(digitalLink.ToString(false), language, linkTypes);
    }
}
