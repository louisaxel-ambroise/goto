using Gs1DigitalLink.Core.Conversion;
using Gs1DigitalLink.Core.Conversion.Utils;
using Gs1DigitalLink.Core.Registration;
using Gs1DigitalLink.Core.Resolution;

namespace Gs1DigitalLink.Core.Tests.Resolution;

[TestClass]
public sealed class DigitalLinkResolverTests
{
    private static DigitalLink CreateDigitalLink()
    {
        return new DigitalLink
        {
            CompanyPrefix = "123456",
            Type = DigitalLinkType.Unknown,
            QueryString = [],
            AIs =
            [
                new KeyValue
                { 
                    Key = new Identifier { Code = "01", Type = AIType.PrimaryKey }, 
                    Components = [ new Component{ Definition = new(){ Length= 14, Type = Charset.Numeric }, Value = "09506000134352" } ],
                    Issues = []
                },
                new KeyValue
                {
                    Key = new Identifier { Code = "10", Type = AIType.Qualifier },
                    Components = [ new Component{ Definition = new(){ Length= 14, Type = Charset.Alpha }, Value = "ABC123" } ],
                    Issues = []
                }
            ]
        };
    }

    [TestMethod]
    public void GetCandidatesTests_ShouldReturnConfiguredLinks()
    {
        var links = new[]
        {
            new Link { Prefix = "01/09506000134352", Language = null, Title = "test gtin", RedirectUrl = "http://a", LinkType = "gs1:defaultLink", Applicability = new(){ From = TimeProvider.System.GetUtcNow() } }
        };

        var resolver = new DigitalLinkResolver(new FakePrefixRegistry(links), new FakeLanguageContext());

        var result = resolver.GetCandidates(CreateDigitalLink(), TimeProvider.System.GetUtcNow(), null).ToList();

        Assert.ContainsSingle(result);
        Assert.AreEqual("http://a", result[0].RedirectUrl);
    }
}
