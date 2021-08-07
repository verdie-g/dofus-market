using System.Threading;
using System.Threading.Tasks;
using Dofus;
using Dofus.Messages;

namespace DofusMarket.Frames
{
    public class QuestFrame : Frame
    {
        public override async Task ProcessAsync(CancellationToken cancellationToken)
        {
            await SendMessageAsync(new QuestListRequestMessage());
        }
    }
}
