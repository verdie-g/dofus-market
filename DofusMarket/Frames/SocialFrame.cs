using System.Threading;
using System.Threading.Tasks;
using Dofus;
using Dofus.Messages;

namespace DofusMarket.Frames
{
    public class SocialFrame : Frame
    {
        public override async Task ProcessAsync(CancellationToken cancellationToken)
        {
            await SendMessageAsync(new FriendsGetListMessage());
            await SendMessageAsync(new AcquaintancesGetListMessage());
            await SendMessageAsync(new IgnoredGetListMessage());
            await SendMessageAsync(new SpouseGetInformationsMessage());
        }
    }
}
