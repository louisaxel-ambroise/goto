using DecoratR;
using Gs1DigitalLink.Core.Conversion;
using Gs1DigitalLink.Core.Conversion.Utils;
using Gs1DigitalLink.Core.Conversion.Utils.Validation;
using Gs1DigitalLink.Core.Insights;
using Gs1DigitalLink.Core.Registration;
using Gs1DigitalLink.Core.Resolution;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

namespace Gs1DigitalLink.Core;

public static class ServiceCollectionExtensions
{
    public static void AddDigitalLinkCore(this IServiceCollection services)
    {
        var jsonOptions = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };

        using (var file = File.OpenRead("Documents/gcpprefixformatlist.xml"))
        {
            CompanyPrefix.Initialize(file);
        }
        using (var file = File.OpenRead("Documents/OptimizationCodes.json"))
        {
            services.AddSingleton(JsonSerializer.Deserialize<OptimizationCodes>(file, jsonOptions) ?? new() { Codes = [] });
        }
        using (var file = File.OpenRead("Documents/ApplicationIdentifiers.json"))
        {
            services.AddSingleton(JsonSerializer.Deserialize<ApplicationIdentifiers>(file, jsonOptions) ?? new() { Identifiers = [], CodeLength = [] });
        }
        services.AddSingleton(TimeProvider.System);
        services.AddSingleton<IDigitalLinkConverter, DigitalLinkConverter>();
        services.AddSingleton<ILinkRegistrator, LinkRegistrator>();
        services.AddSingleton<IInsightRetriever, InsightRetriever>();
        services.Decorate<IDigitalLinkResolver>()
            .With<InsightDigitalLinkResolver>()
            .Then<DigitalLinkResolver>()
            .AsScoped()
            .Apply();
    }
}

