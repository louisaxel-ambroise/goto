namespace Gs1DigitalLink.Core.Registration;

public interface IPrefixRegistry
{
    void Register(string prefix, string title, string redirectUrl, string? language, DateTimeRange applicability, IEnumerable<string> linkTypes);
    void Unregister(string prefix, string? language, IEnumerable<string> linkTypes);
    IEnumerable<Link> Resolve(DateTimeOffset applicability, IEnumerable<string> prefixes);
}
