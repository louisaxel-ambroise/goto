using System.Threading.Channels;

namespace Gs1DigitalLink.Core.Insights;

public interface IInsightRecorder
{
    void Record(ScanInsight insight);
}

public class InsightRecorder(Channel<ScanInsight> channel) : IInsightRecorder
{
    public void Record(ScanInsight insight)
    {
        channel.Writer.TryWrite(insight);
    }
}