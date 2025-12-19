using Gs1DigitalLink.Core.Registration;

namespace Gs1DigitalLink.Core.Tests;

internal sealed class FakePrefixRegistry(IEnumerable<Link> links) : IPrefixRegistry
{
    public IEnumerable<Link> Resolve(string companyPrefix, IEnumerable<string> prefixes)
    {
        return links;
    }

    public void Register(string companyPrefix, string prefix, string title, string redirectUrl, string? language, string linkType)
    {
        throw new NotImplementedException();
    }

    public void Unregister(string companyPrefix, string prefix, string? language, string linkType)
    {
        throw new NotImplementedException();
    }
}
