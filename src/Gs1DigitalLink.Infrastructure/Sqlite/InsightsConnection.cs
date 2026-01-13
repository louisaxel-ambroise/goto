using Dapper;
using Microsoft.Data.Sqlite;

namespace Gs1DigitalLink.Infrastructure.Sqlite;

internal sealed class InsightsConnection : SqliteConnection
{
    public InsightsConnection() : base("Data Source=insights.db")
    {
        Open();
    }

    internal static InsightsConnection Connect()
    {
        var connection = new InsightsConnection();
        connection.Execute("PRAGMA journal_mode = WAL;PRAGMA synchronous = NORMAL;PRAGMA cache_size = -50000;PRAGMA mmap_size = 536870912;PRAGMA temp_store = MEMORY;PRAGMA threads = 4;PRAGMA busy_timeout = 3000;");

        return connection;
    }

    internal static void Initialize()
    {
        using var connection = new InsightsConnection();

        connection.Execute("CREATE TABLE IF NOT EXISTS [Insights](DigitalLink TEXT, Timestamp INTEGER, LinkType TEXT, Languages TEXT, CandidateCount INTEGER);");
    }
}