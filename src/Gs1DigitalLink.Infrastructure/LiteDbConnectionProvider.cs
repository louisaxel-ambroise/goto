using LiteDB;

namespace Gs1DigitalLink.Infrastructure;

internal static class LiteDbConnectionProvider
{
    public static LiteDatabase Connect(string companyPrefix)
    {
        return new($"Filename=GCP_{companyPrefix}.db;Connection=Shared;");
    }
}
