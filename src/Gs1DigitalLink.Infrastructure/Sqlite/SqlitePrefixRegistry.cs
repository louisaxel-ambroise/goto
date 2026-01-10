using Dapper;
using Gs1DigitalLink.Core.Registration;
using System.Data;

namespace Gs1DigitalLink.Infrastructure.Sqlite;

internal sealed class SqlitePrefixRegistry(RegistryConnection connection, TimeProvider timeProvider) : IPrefixRegistry
{
    public void Register(string prefix, string title, string redirectUrl, string? language, DateTimeRange applicability, IEnumerable<string> linkTypes)
    {
        var applicableTo = (applicability.To ?? timeProvider.GetUtcNow()).ToUnixTimeSeconds();
        var links = linkTypes.Select(type => new StoredLink
        {
            Language = language,
            LinkType = type,
            Prefix = prefix,
            RedirectUrl = redirectUrl,
            Title = title,
            ApplicableFrom = applicability.From.ToUnixTimeSeconds(),
            ApplicableTo = applicableTo
        });

        using var transaction = connection.BeginTransaction();

        if (linkTypes.Contains("gs1:defaultLink"))
        {
            connection.Execute("UPDATE [StoredLinks] SET ApplicableTo = @ApplicableTo WHERE Prefix = @Prefix AND LinkType = @LinkType;", new { Prefix = prefix, LinkType = "gs1:defaultLink", ApplicableTo = applicableTo }, transaction);
        }

        connection.Execute("INSERT INTO [StoredLinks](Prefix, RedirectUrl, Title, Language, LinkType, ApplicableFrom, ApplicableTo) SELECT Prefix, RedirectUrl, Title, Language, LinkType, @ApplicableTo, ApplicableTo FROM [StoredLinks] WHERE Prefix = @Prefix AND Language = @Language AND LinkType = @LinkType AND ApplicableFrom < @ApplicableFrom AND ApplicableTo > @ApplicableTo;", links, transaction);
        connection.Execute("UPDATE [StoredLinks] SET ApplicableTo = MIN(ApplicableTo, @ApplicableFrom) WHERE Prefix = @Prefix AND Language = @Language AND LinkType = @LinkType AND ApplicableFrom < @ApplicableFrom;", links, transaction);
        connection.Execute("UPDATE [StoredLinks] SET ApplicableFrom = MAX(ApplicableFrom, @ApplicableTo) WHERE Prefix = @Prefix AND Language = @Language AND LinkType = @LinkType AND ApplicableFrom > @ApplicableTo;", links, transaction);
        connection.Execute("INSERT INTO [StoredLinks](Prefix, RedirectUrl, Title, Language, LinkType, ApplicableFrom, ApplicableTo) VALUES(@Prefix, @RedirectUrl, @Title, @Language, @LinkType, @ApplicableFrom, @ApplicableTo);", links, transaction);
        transaction.Commit();
    }

    public void Unregister(string prefix, string? language, IEnumerable<string> linkTypes)
    {
        var applicableTo = timeProvider.GetUtcNow().ToUnixTimeSeconds();

        connection.Execute("UPDATE [StoredLinks] SET ApplicableTo = @ApplicableTo WHERE Prefix = @Prefix AND LinkType = @Language AND LinkType IN @LinkTypes AND ApplicableFrom < @ApplicableTo AND (ApplicableTo IS NULL OR ApplicableTo > @ApplicableTo);", new { Prefix = prefix, Language = language, LinkTypes = linkTypes, ApplicableTo = applicableTo });
    }

    public IEnumerable<Link> Resolve(DateTimeOffset applicability, IEnumerable<string> prefixes)
    {
        var applicabilityDate = applicability.ToUnixTimeSeconds();
        var links = connection.Query<StoredLink>("SELECT Prefix, RedirectUrl, Title, Language, LinkType, ApplicableFrom FROM [StoredLinks] WHERE Prefix IN @Prefixes AND ApplicableFrom <= @Applicability AND (ApplicableTo IS NULL OR ApplicableTo > @Applicability)", new { Prefixes = prefixes, Applicability = applicabilityDate });

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
            Title = link.Title,
            Applicability = ParseRange(link.ApplicableFrom, link.ApplicableTo)
        });
    }

    private static DateTimeRange ParseRange(long applicableFrom, long? applicableTo)
    {
        var from = DateTimeOffset.FromUnixTimeSeconds(applicableFrom);
        var to = applicableTo is null ? default(DateTimeOffset?) : DateTimeOffset.FromUnixTimeSeconds(applicableTo.Value);

        return new() { From = from, To = to };
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
    public required long ApplicableFrom { get; init; }
    public long? ApplicableTo { get; init; }
}