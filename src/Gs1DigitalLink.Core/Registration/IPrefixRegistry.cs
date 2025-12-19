namespace Gs1DigitalLink.Core.Registration;

public interface IPrefixRegistry
{
    void Register(string companyPrefix, string prefix, string title, string redirectUrl, string? language, string linkType);
    void Unregister(string companyPrefix, string prefix, string? language, string linkType);
    IEnumerable<Link> Resolve(string companyPrefix, IEnumerable<string> prefixes);
}
