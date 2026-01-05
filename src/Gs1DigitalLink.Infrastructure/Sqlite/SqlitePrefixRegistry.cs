using Dapper;
using Gs1DigitalLink.Core.Registration;
using LiteDB;
using System.Data;

namespace Gs1DigitalLink.Infrastructure;

internal sealed class SqlitePrefixRegistry(IDbConnection connection) : IPrefixRegistry
{
    public void Register(string prefix, string title, string redirectUrl, string? language, IEnumerable<string> linkTypes)
    {
        var links = linkTypes.Select(type => new StoredLink
        {
            Language = language,
            LinkType = type,
            Prefix = prefix,
            RedirectUrl = redirectUrl,
            Title = title
        });

        using var transaction = connection.BeginTransaction();

        if (linkTypes.Contains("gs1:defaultLink"))
        {
            connection.Execute("DELETE FROM [StoredLinks] WHERE Prefix = @Prefix AND LinkType = @LinkType", new { Prefix = prefix, LinkType = "gs1:defaultLink" }, transaction);
        }

        connection.Execute("INSERT OR REPLACE INTO [StoredLinks](Prefix, RedirectUrl, Title, Language, LinkType) VALUES(@Prefix, @RedirectUrl, @Title, @Language, @LinkType)", links, transaction);
        transaction.Commit();
    }

    public void Unregister(string prefix, string? language, IEnumerable<string> linkTypes)
    {
        connection.Execute("DELETE FROM [StoredLinks] WHERE Prefix = @Prefix AND Language = @Language AND LinkType IN @LinkTypes", new { Prefix = prefix, Language = language, LinkTypes = linkTypes });
    }

    public IEnumerable<Link> Resolve(IEnumerable<string> prefixes)
    {
        var links = connection.Query<StoredLink>("SELECT Prefix, RedirectUrl, Title, Language, LinkType FROM [StoredLinks] WHERE Prefix IN @Prefixes", new { Prefixes = prefixes });

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