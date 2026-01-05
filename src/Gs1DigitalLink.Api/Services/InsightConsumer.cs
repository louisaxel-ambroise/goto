using Gs1DigitalLink.Core.Insights;
using System.Threading.Channels;

namespace Gs1DigitalLink.Api.Services;

internal sealed class InsightConsumer(Channel<ScanInsight> channel, IServiceProvider serviceProvider) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var insight in channel.Reader.ReadAllAsync(stoppingToken))
        {
            try
            {
                using var scope = serviceProvider.CreateScope();
                var sink = scope.ServiceProvider.GetRequiredService<IInsightSink>();

                sink.Store(insight);
            }
            catch
            {
                // log + continue
            }
        }
    }
}