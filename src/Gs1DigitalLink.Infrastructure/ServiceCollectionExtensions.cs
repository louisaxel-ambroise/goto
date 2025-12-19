using Gs1DigitalLink.Core.Insights;
using Gs1DigitalLink.Core.Registration;
using Microsoft.Extensions.DependencyInjection;

namespace Gs1DigitalLink.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static void AddDigitalLinkInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IPrefixRegistry, LiteDbPrefixRegistry>();
        services.AddSingleton<IInsightSink, LiteDbInsightSink>();
    }
}
