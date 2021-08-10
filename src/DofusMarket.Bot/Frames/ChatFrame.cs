using System.Threading;
using System.Threading.Tasks;
using Dofus;
using Dofus.Messages;

namespace DofusMarket.Bot.Frames
{
    public class ChatFrame : Frame
    {
        public override async Task ProcessAsync(CancellationToken cancellationToken)
        {
            await SendMessageAsync(new ChannelEnablingMessage { Channel = 7, Enable = false });
            await SendMessageAsync(new ChannelEnablingMessage { Channel = 14, Enable = false });
            await SendMessageAsync(new ChannelEnablingMessage { Channel = 16, Enable = true });
        }
    }
}
