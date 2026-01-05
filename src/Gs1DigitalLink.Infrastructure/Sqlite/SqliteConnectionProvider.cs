using Dapper;
using Microsoft.Data.Sqlite;
using System.Data;

namespace Gs1DigitalLink.Infrastructure;

internal static class SqliteConnectionProvider
{
    public static void Initialize()
    {
        using var connection = Connect();

        connection.Execute("CREATE TABLE IF NOT EXISTS [Insights](DigitalLink TEXT, Timestamp INTEGER, LinkType TEXT, Languages TEXT, CandidateCount INTEGER);");
        connection.Execute("CREATE TABLE IF NOT EXISTS [StoredLinks](Prefix TEXT, RedirectUrl TEXT, Title TEXT, Language TEXT, LinkType TEXT);");
    }

    public static IDbConnection Connect()
    {
        var connection = new SqliteConnection("Data Source=sqlite.db");
        connection.Open();

        connection.Execute("PRAGMA journal_mode = WAL;PRAGMA synchronous = NORMAL;PRAGMA cache_size = -50000;PRAGMA mmap_size = 536870912;PRAGMA temp_store = MEMORY;PRAGMA threads = 4;PRAGMA busy_timeout = 3000;");

        return connection;
    }
}