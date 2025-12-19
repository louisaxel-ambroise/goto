using Gs1DigitalLink.Core.Registration;
using LiteDB;

namespace Gs1DigitalLink.Infrastructure;

internal sealed class LiteDbPrefixRegistry : IPrefixRegistry
{
    public void Register(string companyPrefix, string prefix, string title, string redirectUrl, string? language, string linkType)
    {
        using var connection = LiteDbConnectionProvider.Connect(companyPrefix);

        var collection = connection.GetCollection<StoredLink>();
        var registeredLink = collection.Query()
            .Where(l => l.Prefix == prefix)
            .Where(l => l.Language == language)
            .Where(l => l.LinkType == linkType)
            .FirstOrDefault();

        if (registeredLink is not null)
        {
            registeredLink = registeredLink with
            {
                RedirectUrl = redirectUrl,
                Title = title
            };
        }
        else
        {
            registeredLink = new StoredLink
            {
                Language = language,
                LinkType = linkType,
                Prefix = prefix, 
                RedirectUrl = redirectUrl,
                Title = title
            };
        }

        collection.Upsert(registeredLink);

        if (linkType == "gs1:defaultLink")
        {
            collection.DeleteMany(l => l.LinkType == "gs1:defaultLink" && l.Id != registeredLink.Id);
        }
    }

    public void Unregister(string companyPrefix, string prefix, string? language, string linkType)
    {
        using var connection = LiteDbConnectionProvider.Connect(companyPrefix);

        var collection = connection.GetCollection<StoredLink>();

        collection.DeleteMany(l => l.LinkType == linkType && l.Language == language && l.Prefix == prefix);
    }

    public IEnumerable<Link> Resolve(string companyPrefix, IEnumerable<string> prefixes)
    {
        using var connection = LiteDbConnectionProvider.Connect(companyPrefix);

        var collection = connection.GetCollection<StoredLink>();
        var links = collection.Query().Where(l => prefixes.Contains(l.Prefix)).ToList();

        return MapToLinks(links.GroupBy(l => new { l.LinkType, l.Language }).Select(g => g.OrderByDescending(l => l.Prefix.Length).First()));
    }

    private static IEnumerable<Link> MapToLinks(IEnumerable<StoredLink> results)
    {
        return results.Select(link => new Link
            {
                Language = link.Language,
                LinkType = link.LinkType,
                RedirectUrl = link.RedirectUrl,
                Prefix = link.Prefix,
                Title = link.Title
            });
    }
}

public sealed record StoredLink
{
    public int Id { get; init; }
    public required string Prefix { get; init; }
    public required string RedirectUrl { get; init; }
    public required string Title { get; init; }
    public required string? Language { get; init; }
    public required string LinkType { get; init; }
}