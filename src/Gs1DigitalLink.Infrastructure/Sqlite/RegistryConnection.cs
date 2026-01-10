using Dapper;
using Microsoft.Data.Sqlite;

namespace Gs1DigitalLink.Infrastructure.Sqlite;

internal sealed class RegistryConnection : SqliteConnection
{
    private RegistryConnection() : base("Data Source=registry.db")
    {
        Open();
    }

    internal static RegistryConnection Connect()
    {
        var connection = new RegistryConnection();
        connection.Execute("PRAGMA journal_mode = WAL;PRAGMA synchronous = NORMAL;PRAGMA cache_size = -50000;PRAGMA mmap_size = 536870912;PRAGMA temp_store = MEMORY;PRAGMA threads = 4;PRAGMA busy_timeout = 3000;");

        return connection;
    }

    internal static void Initialize()
    {
        using var connection = new RegistryConnection();
        
        connection.Execute("CREATE TABLE IF NOT EXISTS [StoredLinks](Id INT PRIMARY KEY, Prefix TEXT, RedirectUrl TEXT, Title TEXT, Language TEXT, LinkType TEXT, ApplicableFrom INT, ApplicableTo INT NULL, InsertedOn INT NULL, UpdatedOn INT NULL);");
    }
}
