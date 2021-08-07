using System.Threading;
using System.Threading.Tasks;
using Dofus;
using Dofus.Messages;
using Dofus.Types;

namespace DofusMarket.Frames
{
    public class AllianceFrame : Frame
    {
        public override async Task ProcessAsync(CancellationToken cancellationToken)
        {
            await SendMessageAsync(new PrismsListRegisterMessage { Listen = PrismListen.All });
        }
    }
}
