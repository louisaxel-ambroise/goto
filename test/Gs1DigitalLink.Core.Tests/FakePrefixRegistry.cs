using Gs1DigitalLink.Core.Registration;

namespace Gs1DigitalLink.Core.Tests;

internal sealed class FakePrefixRegistry(IEnumerable<Link> links) : IPrefixRegistry
{
    public IEnumerable<Link> Resolve(DateTimeOffset applicability, IEnumerable<string> prefixes)
    {
        return links;
    }

    public void Register(string prefix, string title, string redirectUrl, string? language, DateTimeRange applicability, IEnumerable<string> linkTypes)
    {
        throw new NotImplementedException();
    }

    public void Unregister(string prefix, string? language, IEnumerable<string> linkTypes)
    {
        throw new NotImplementedException();
    }
}
