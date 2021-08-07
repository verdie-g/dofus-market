using System.Threading;
using System.Threading.Tasks;
using Dofus;
using Dofus.Messages;
using Microsoft.Extensions.Logging;

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

            while (true)
            {
                var message = await ReceiveMessageAsync<ChatServerMessage>();
                Logger.LogInformation("[CHAT] {0}: {1}", message.SenderName, message.Content);
            }
        }
    }
}
