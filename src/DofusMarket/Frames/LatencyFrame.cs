using System;
using System.Threading;
using System.Threading.Tasks;
using Dofus;
using Dofus.Messages;

namespace DofusMarket.Frames
{
    internal class LatencyFrame : Frame
    {
        public override async Task ProcessAsync(CancellationToken cancellationToken)
        {
            short sampleCount = 0;
            while (true)
            {
                var message = await ReceiveMessageAsync<INetworkMessage>();
                sampleCount = (short)Math.Min(sampleCount + 1, 50);

                if (message is BasicLatencyStatsRequestMessage)
                {
                    await SendMessageAsync(new BasicLatencyStatsMessage
                    {
                        Latency = 24,
                        SampleCount = sampleCount,
                        Max = 50,
                    });
                }
            }
        }
    }
}
