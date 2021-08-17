using System.Threading;
using System.Threading.Tasks;
using Dofus;
using Dofus.Messages;
using Microsoft.Extensions.Logging;

namespace DofusMarket.Bot.Frames
{
    internal class MiscFrame : Frame
    {
        public override async Task ProcessAsync(CancellationToken cancellationToken)
        {
            while (cancellationToken.IsCancellationRequested)
            {
                var checkFileRequest = await ReceiveMessageAsync<CheckFileRequestMessage>();
                Logger.LogError("Received CheckFileRequestMessage for {0} ({1}", checkFileRequest.Filename, checkFileRequest.Type);
            }
        }
    }
}
