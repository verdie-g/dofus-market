using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Diagnostics;
using System.IO;
using System.IO.Pipelines;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Dofus.Extensions;
using Dofus.Messages;
using Dofus.Serialization;
using Microsoft.Extensions.Logging;

namespace Dofus
{
    public class DofusClient : IDisposable
    {
        private const int HeaderLengthBitSize = 2;
        private const int HeaderLengthSizeMask = 0x03;

        private readonly EndPoint _endPoint;
        private readonly ILogger _logger;
        private readonly Socket _socket;
        private readonly SemaphoreSlim _socketSemaphore;
        private readonly Channel<INetworkMessage> _messagesChannel;
        private readonly Pipe _socketPipe;
        private readonly CancellationTokenSource _socketPipeCancellation;

        private int _sequenceId;

        public DofusClient(EndPoint endPoint, ILogger logger)
        {
            _endPoint = endPoint;
            _logger = logger;
            _socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp) { NoDelay = true };
            _socketSemaphore = new SemaphoreSlim(1);
            _messagesChannel = Channel.CreateUnbounded<INetworkMessage>(new UnboundedChannelOptions
            {
                SingleReader = true,
                SingleWriter = true,
                AllowSynchronousContinuations = false,
            });

            _socketPipe = new(new PipeOptions());
            _socketPipeCancellation = new CancellationTokenSource();
            _ = FillPipeAsync(_socketPipeCancellation.Token);
            _ = ReadPipeAsync(_socketPipeCancellation.Token);
        }

        public ChannelReader<INetworkMessage> Messages => _messagesChannel.Reader;

        public Task SendMessageAsync(INetworkMessage message)
        {
            _logger.LogDebug("Sending message {0}", message.GetType().Name);

            using var stream = new MemoryStream();
            using (var writer = new DofusBinaryWriter(stream, leaveOpen: true))
            {
                try
                {
                    message.Serialize(writer);
                }
                catch (Exception e)
                {
                    throw new SerializationException($"An error occured serializing message {message.GetType().Name}", e);
                }
            }

            byte[] messageContent = stream.ToArray();
            int lengthSize = ComputeMessageLengthSize(messageContent.Length);

            stream.SetLength(0);
            using (var writer = new DofusBinaryWriter(stream, leaveOpen: true))
            {
                int messageId = NetworkMessageRegistry.GetIdFromType(message.GetType());
                writer.Write((short)((messageId << HeaderLengthBitSize) | lengthSize));

                writer.Write(Interlocked.Increment(ref _sequenceId));

                Span<byte> lengthBytes = stackalloc byte[sizeof(int)];
                BinaryPrimitives.WriteInt32BigEndian(lengthBytes, messageContent.Length);
                writer.Write(lengthBytes.Slice(lengthBytes.Length - lengthSize));

                writer.Write(messageContent);
            }

            return SendAsync(stream.ToArray());
        }

        public void Dispose()
        {
            _socketPipeCancellation.Cancel();
            _socket.Dispose();
        }

        private async Task SendAsync(ArraySegment<byte> buffer)
        {
            await _socketSemaphore.WaitAsync();
            try
            {
                await _socket.SendAsync(buffer, SocketFlags.None);
            }
            finally
            {
                _socketSemaphore.Release();
            }
        }

        private async Task FillPipeAsync(CancellationToken cancellationToken)
        {
            await _socket.ConnectAsync(_endPoint, cancellationToken);

            var pipeWriter = _socketPipe.Writer;
            while (true)
            {
                Memory<byte> memory = pipeWriter.GetMemory(2048);
                try
                {
                    int bytesRead = await _socket.ReceiveAsync(memory, SocketFlags.None, cancellationToken);
                    if (bytesRead == 0)
                    {
                        _logger.LogInformation("Connection was closed");
                        break;
                    }

                    pipeWriter.Advance(bytesRead);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "An error occured while reading from socket");
                    break;
                }

                FlushResult result = await pipeWriter.FlushAsync(cancellationToken);
                if (result.IsCompleted)
                {
                    _logger.LogInformation("Message channel was closed");
                    break;
                }
            }

            await pipeWriter.CompleteAsync();
            _messagesChannel.Writer.Complete();
        }

        private async Task ReadPipeAsync(CancellationToken cancellationToken)
        {
            var pipeReader = _socketPipe.Reader;
            while (true)
            {
                ReadResult result = await pipeReader.ReadAsync(cancellationToken);
                var buffer = result.Buffer;

                SequencePosition consumed;
                try
                {
                    consumed = ReadAndHandleMessages(in buffer);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "An error occured processing the pipe");
                    break;
                }

                if (result.IsCompleted)
                {
                    break;
                }

                pipeReader.AdvanceTo(consumed, buffer.End);
            }

            await pipeReader.CompleteAsync();
        }

        private SequencePosition ReadAndHandleMessages(in ReadOnlySequence<byte> sequence)
        {
            SequenceReader<byte> sequenceReader = new(sequence);
            SequencePosition consumed = sequenceReader.Position;
            while (!sequenceReader.End && TryReadMessage(ref sequenceReader, out RawNetworkMessage message))
            {
                HandleMessage(message);
                consumed = sequenceReader.Position;
            }

            return consumed;
        }

        private bool TryReadMessage(ref SequenceReader<byte> sequenceReader, out RawNetworkMessage rawMessage)
        {
            if (!sequenceReader.TryReadBigEndian(out short messageHeader))
            {
                rawMessage = default;
                return false;
            }

            ushort messageId = GetMessageId(messageHeader);
            if (!ReadMessageLength(ref sequenceReader, messageHeader, out int messageLength))
            {
                rawMessage = default;
                return false;
            }

            byte[] messageContent = new byte[messageLength];
            if (!sequenceReader.TryRead(messageContent))
            {
                rawMessage = default;
                return false;
            }

            rawMessage = new RawNetworkMessage(messageId, messageContent);
            return true;
        }

        private void HandleMessage(RawNetworkMessage rawMessage)
        {
            Interlocked.Increment(ref _sequenceId);

            if (!NetworkMessageRegistry.TryGetTypeFromId(rawMessage.Id, out Type? messageType))
            {
                _logger.LogWarning("Ignoring unknown message id '{0}'", rawMessage.Id);
                return;
            }

            _logger.LogDebug("Received message {0}", messageType.Name);
            var message = (INetworkMessage)Activator.CreateInstance(messageType)!;
            try
            {
                using var stream = new MemoryStream(rawMessage.Content);
                using var reader = new DofusBinaryReader(stream);
                message.Deserialize(reader);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occured deserializing message {0}", messageType.Name);
                return;
            }

            bool written = _messagesChannel.Writer.TryWrite(message);
            Debug.Assert(written);
        }

        private ushort GetMessageId(short messageHeader)
        {
            return (ushort)((ushort)messageHeader >> HeaderLengthBitSize);
        }

        private bool ReadMessageLength(ref SequenceReader<byte> sequenceReader, short messageHeader, out int length)
        {
            int lengthSize = messageHeader & HeaderLengthSizeMask;
            if (lengthSize == 0)
            {
                length = 0;
                return true;
            }

            var lengthBytes = new byte[sizeof(int)];
            if (!sequenceReader.TryRead(lengthBytes.AsSpan(lengthBytes.Length - lengthSize)))
            {
                length = 0;
                return false;
            }

            length = BinaryPrimitives.ReadInt32BigEndian(lengthBytes);
            return true;
        }

        private int ComputeMessageLengthSize(int length)
        {
            return length switch
            {
                > 0xFFFF => 3,
                > 0xFF => 2,
                > 0 => 1,
                _ => 0,
            };
        }
    }
}
