using System;
using System.Threading;
using System.Threading.Tasks;
using Dofus;
using Dofus.Messages;

namespace DofusMarket.Frames
{
    internal class LatencyFrame : Frame
    {
        private readonly Random _rnd = new();

        public override async Task ProcessAsync(CancellationToken cancellationToken)
        {
            while (true)
            {
                await ReceiveMessageAsync<BasicLatencyStatsRequestMessage>();
                continue;
                await SendMessageAsync(new BasicLatencyStatsMessage
                {
                    Latency = (short)_rnd.Next(20, 120),
                    SampleCount = 50,
                    Max = 50,
                });
            }
        }
    }
}
