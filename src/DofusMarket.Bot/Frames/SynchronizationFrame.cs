using System.Threading;
using System.Threading.Tasks;
using Dofus;
using Dofus.Messages;

namespace DofusMarket.Bot.Frames
{
    public class SynchronizationFrame : Frame
    {
        public override async Task ProcessAsync(CancellationToken cancellationToken)
        {
            ushort sequenceNumber = 1;
            while (true)
            {
                await ReceiveMessageAsync<SequenceNumberRequestMessage>();
                await SendMessageAsync(new SequenceNumberMessage { Number = sequenceNumber });
                sequenceNumber += 1;
            }
        }
    }
}
