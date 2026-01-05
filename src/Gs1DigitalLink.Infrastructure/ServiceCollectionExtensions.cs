using Gs1DigitalLink.Core.Insights;
using Gs1DigitalLink.Core.Registration;
using Microsoft.Extensions.DependencyInjection;

namespace Gs1DigitalLink.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static void AddDigitalLinkInfrastructure(this IServiceCollection services)
    {
        SqliteConnectionProvider.Initialize();

        services.AddScoped(_ => SqliteConnectionProvider.Connect());
        services.AddScoped<IPrefixRegistry, SqlitePrefixRegistry>();
        services.AddScoped<IInsightSink, SqliteInsightSink>();
    }
}
