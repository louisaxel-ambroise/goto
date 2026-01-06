using Dapper;
using Microsoft.Data.Sqlite;

namespace Gs1DigitalLink.Infrastructure.Sqlite;

internal sealed class DigitalLinkConnection : SqliteConnection
{
    private DigitalLinkConnection() : base("Data Source=registry.db")
    {
        Open();
    }

    internal static DigitalLinkConnection Connect()
    {
        var connection = new DigitalLinkConnection();
        connection.Execute("PRAGMA journal_mode = WAL;PRAGMA synchronous = NORMAL;PRAGMA cache_size = -50000;PRAGMA mmap_size = 536870912;PRAGMA temp_store = MEMORY;PRAGMA threads = 4;PRAGMA busy_timeout = 3000;");

        return connection;
    }

    internal static void Initialize()
    {
        using var connection = new DigitalLinkConnection();
        
        connection.Execute("CREATE TABLE IF NOT EXISTS [StoredLinks](Prefix TEXT, RedirectUrl TEXT, Title TEXT, Language TEXT, LinkType TEXT);");
    }
}
