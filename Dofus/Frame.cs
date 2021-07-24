using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Dofus.Messages;
using Microsoft.Extensions.Logging;

namespace Dofus
{
    public abstract class Frame
    {
        private DofusClient _client = default!;
        protected ILogger Logger = default!;
        internal Channel<INetworkMessage> Messages = default!;

        /// <summary>Task returned by <see cref="ProcessTask"/>.</summary>
        public Task ProcessTask { get; internal set; } = default!;

        public abstract Task ProcessAsync(CancellationToken cancellationToken);

        internal void Initialize(Channel<INetworkMessage> messages, DofusClient client, ILogger logger)
        {
            Messages = messages;
            _client = client;
            Logger = logger;
        }

        protected Task SendMessageAsync(INetworkMessage message)
        {
            return _client!.SendMessageAsync(message);
        }

        protected async Task<TMessage> ReceiveMessageAsync<TMessage>() where TMessage : INetworkMessage
        {
            return (TMessage)await ReceiveAnyMessageAsync(typeof(TMessage));
        }

        protected async Task<INetworkMessage> ReceiveAnyMessageAsync(params Type[] messageTypes)
        {
            await foreach (INetworkMessage message in Messages.Reader.ReadAllAsync())
            {
                Type messageType = message.GetType();
                foreach (var expectedMessageType in messageTypes)
                {
                    if (expectedMessageType.IsAssignableFrom(messageType))
                    {
                        return message;
                    }
                }
            }

            throw new OperationCanceledException();
        }
    }
}
