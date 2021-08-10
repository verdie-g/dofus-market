using System.Threading;
using System.Threading.Tasks;
using Dofus;
using Dofus.Messages;

namespace DofusMarket.Bot.Frames
{
    public class WorldFrame : Frame
    {
        public override async Task ProcessAsync(CancellationToken cancellationToken)
        {
            await SendMessageAsync(new AnomalySubareaInformationRequestMessage());
        }
    }
}
