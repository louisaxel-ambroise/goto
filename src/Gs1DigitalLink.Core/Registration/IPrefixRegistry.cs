namespace Gs1DigitalLink.Core.Registration;

public interface IPrefixRegistry
{
    void Register(string prefix, string title, string redirectUrl, string? language, IEnumerable<string> linkTypes);
    void Unregister(string prefix, string? language, IEnumerable<string> linkTypes);
    IEnumerable<Link> Resolve(IEnumerable<string> prefixes);
}
