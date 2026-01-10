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
        RegistryConnection.Initialize();

        services.AddScoped(_ => RegistryConnection.Connect());
        services.AddScoped(_ => InsightsConnection.Connect());
        services.AddScoped<IPrefixRegistry, SqlitePrefixRegistry>();
        services.AddScoped<IInsightSink, SqliteInsightSink>();
    }
}
