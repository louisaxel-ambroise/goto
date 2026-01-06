using Gs1DigitalLink.Core.Insights;
using Gs1DigitalLink.Core.Registration;
using Gs1DigitalLink.Infrastructure.Sqlite;
using Microsoft.Extensions.DependencyInjection;

namespace Gs1DigitalLink.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static void AddDigitalLinkInfrastructure(this IServiceCollection services)
    {
        InsightsConnection.Initialize();
        DigitalLinkConnection.Initialize();

        services.AddScoped(_ => DigitalLinkConnection.Connect());
        services.AddScoped(_ => InsightsConnection.Connect());
        services.AddScoped<IPrefixRegistry, SqlitePrefixRegistry>();
        services.AddScoped<IInsightSink, SqliteInsightSink>();
    }
}
