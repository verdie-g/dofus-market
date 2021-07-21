using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Dofus.Messages;
using Microsoft.Extensions.Logging;

namespace Dofus
{
    /// <summary>Get the messages from a <see cref="DofusClient"/> and push them to all <see cref="Frame"/>s.</summary>
    public class FrameManager : IDisposable
    {
        private readonly DofusClient _client;
        private readonly ILoggerFactory _loggerFactory;
        private readonly List<Frame> _registeredFrames;
        private readonly CancellationTokenSource _framesCancellationSource;

        public FrameManager(DofusClient client, ILoggerFactory loggerFactory)
        {
            _client = client;
            _loggerFactory = loggerFactory;
            _registeredFrames = new List<Frame>();
            _framesCancellationSource = new CancellationTokenSource();

            Task.Run(DistributeMessagesAsync);
        }

        public Frame Register(Frame frame)
        {
            lock (_registeredFrames)
            {
                _registeredFrames.Add(frame);
                frame.Initialize(Channel.CreateUnbounded<INetworkMessage>(new UnboundedChannelOptions
                {
                    SingleReader = true,
                    SingleWriter = true,
                    AllowSynchronousContinuations = false,
                }), _client, _loggerFactory.CreateLogger(frame.GetType()));
                frame.ProcessTask = Task.Run(async () =>
                {
                    try
                    {
                        await frame.ProcessAsync(_framesCancellationSource.Token);
                    }
                    finally
                    {
                        Unregister(frame);
                    }
                });
                return frame;
            }
        }

        public void Unregister(Frame frame)
        {
            lock (_registeredFrames)
            {
                _registeredFrames.Remove(frame);
            }
        }

        public void Dispose()
        {
            _framesCancellationSource.Cancel();
        }

        private async Task DistributeMessagesAsync()
        {
            await foreach (var message in _client.Messages.ReadAllAsync(_framesCancellationSource.Token))
            {
                lock (_registeredFrames)
                {
                    foreach (var frame in _registeredFrames)
                    {
                        Debug.Assert(frame.Messages.Writer.TryWrite(message));
                    }
                }
            }
        }
    }
}
