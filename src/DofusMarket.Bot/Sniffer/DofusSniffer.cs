using System.Buffers;
using System.Buffers.Binary;
using System.Diagnostics;
using System.IO.Pipelines;
using System.Net;
using System.Net.Sockets;
using System.Threading.Channels;
using DofusMarket.Bot.Logging;
using DofusMarket.Bot.Serialization;
using Microsoft.Extensions.Logging;
using PacketDotNet;
using SharpPcap;

namespace DofusMarket.Bot.Sniffer;

internal class DofusSniffer : IDisposable
{
    // From connection.host in config.xml.
    private const string DofusLoginServerHostName = "dofus2-co-production.ankama-games.com";
    private const string DofusGameServersIpRange = "172.65.0.0/16";
    private const int DofusServersPort = 5555;

    private const int HeaderLengthBitSize = 2;
    private const int HeaderLengthSizeMask = 0x03;

    private static readonly ILogger Logger = LoggerProvider.CreateLogger<DofusSniffer>();

    private readonly string _networkDeviceId;
    private readonly Pipe _incomingTcpPipe;
    private readonly Pipe _outgoingTcpPipe;
    private readonly CancellationTokenSource _tcpPipeCancellation;
    private readonly Channel<INetworkMessage> _messagesChannel;
    private ILiveDevice? _device;

    public DofusSniffer(string networkDeviceId)
    {
        _networkDeviceId = networkDeviceId;
        _incomingTcpPipe = new Pipe();
        _outgoingTcpPipe = new Pipe();
        _tcpPipeCancellation = new CancellationTokenSource();
        _messagesChannel = Channel.CreateUnbounded<INetworkMessage>(new UnboundedChannelOptions
        {
            SingleReader = true,
            SingleWriter = true,
            AllowSynchronousContinuations = false,
        });
    }

    public IAsyncEnumerable<INetworkMessage> Messages => _messagesChannel.Reader.ReadAllAsync();

    public DofusSniffer Start()
    {
        _device = CaptureDeviceList.Instance.FirstOrDefault(d => d.Name == $"\\Device\\NPF_{{{_networkDeviceId}}}");
        if (_device == null)
        {
            string availableDevices = string.Join(", ", CaptureDeviceList.Instance.Select(d => $"{d.Name} ({d.Description})"));
            throw new Exception($"No device found with id '{_networkDeviceId}'. Available devices: {availableDevices}");
        }

        Task.Run(() => FillPipesAsync(_tcpPipeCancellation.Token));
        Task.Run(() => ReadPipeAsync(_incomingTcpPipe.Reader, NetworkPacketDirection.Incoming, _tcpPipeCancellation.Token));
        Task.Run(() => ReadPipeAsync(_outgoingTcpPipe.Reader, NetworkPacketDirection.Outgoing, _tcpPipeCancellation.Token));

        return this;
    }

    public void Dispose()
    {
        if (_device != null)
        {
            _device.StopCapture();
            _device.Dispose();
        }
    }

    private async Task FillPipesAsync(CancellationToken cancellationToken)
    {
        Debug.Assert(_device != null);

        var loginServerIpAddresses = await Dns.GetHostAddressesAsync(DofusLoginServerHostName, cancellationToken);

        // Use MaxResponsiveness other it can take up to a second to receive a message.
        _device.Open(DeviceModes.MaxResponsiveness);
        _device.Filter = $"tcp and (port {DofusServersPort}) and (net {DofusGameServersIpRange} or host {string.Join<IPAddress>(" or host ", loginServerIpAddresses)})";

        Logger.LogInformation("Starting the sniffer filtering on \"{0}\"", _device.Filter);

        var incomingTcpPipeWriter = _incomingTcpPipe.Writer;
        var outgoingTcpPipeWriter = _outgoingTcpPipe.Writer;
        GetPacketStatus packetStatus;
        do
        {
            packetStatus = GetNextTcpPacketPayload(out byte[] tcpPayload, out var packetDirection);
            if (packetStatus == GetPacketStatus.PacketRead)
            {
                await (packetDirection == NetworkPacketDirection.Incoming
                    ? incomingTcpPipeWriter.WriteAsync(tcpPayload, cancellationToken)
                    : outgoingTcpPipeWriter.WriteAsync(tcpPayload, cancellationToken));
            }
        } while (packetStatus is GetPacketStatus.PacketRead or GetPacketStatus.ReadTimeout);

        Logger.LogError("Error reading packet. Shutting down the sniffer");

        await incomingTcpPipeWriter.CompleteAsync();
        await outgoingTcpPipeWriter.CompleteAsync();
    }

    private GetPacketStatus GetNextTcpPacketPayload(out byte[] tcpPayload, out NetworkPacketDirection packetDirection)
    {
        Debug.Assert(_device != null);

        var packetStatus = _device.GetNextPacket(out PacketCapture e);
        if (packetStatus != GetPacketStatus.PacketRead)
        {
            tcpPayload = Array.Empty<byte>();
            packetDirection = default;
            return packetStatus;
        }

        var rawPacket = e.GetPacket();
        var packet = Packet.ParsePacket(rawPacket.LinkLayerType, rawPacket.Data);

        var ipPacket = packet.Extract<IPPacket>();
        packetDirection = ipPacket.DestinationAddress.AddressFamily == AddressFamily.InterNetwork
                          && ipPacket.DestinationAddress.GetAddressBytes()[0] == 10
            ? NetworkPacketDirection.Incoming
            : NetworkPacketDirection.Outgoing;

        var tcpPacket = packet.Extract<TcpPacket>();
        tcpPayload = tcpPacket.PayloadData;

        return packetStatus;
    }

    private async Task ReadPipeAsync(PipeReader pipeReader, NetworkPacketDirection packetDirection, CancellationToken cancellationToken)
    {
        while (true)
        {
            ReadResult result = await pipeReader.ReadAsync(cancellationToken);
            var buffer = result.Buffer;

            SequencePosition consumed;
            try
            {
                consumed = ReadAndHandleMessages(in buffer, packetDirection);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "An error occured processing the pipe");
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

    private SequencePosition ReadAndHandleMessages(in ReadOnlySequence<byte> sequence,
        NetworkPacketDirection packetDirection)
    {
        SequenceReader<byte> sequenceReader = new(sequence);
        SequencePosition consumed = sequenceReader.Position;
        while (!sequenceReader.End && TryReadMessage(ref sequenceReader, packetDirection, out RawNetworkMessage message))
        {
            HandleMessage(message, packetDirection);
            consumed = sequenceReader.Position;
        }

        return consumed;
    }

    private bool TryReadMessage(ref SequenceReader<byte> sequenceReader, NetworkPacketDirection packetDirection,
        out RawNetworkMessage rawMessage)
    {
        if (!sequenceReader.TryReadBigEndian(out short messageHeader))
        {
            rawMessage = default;
            return false;
        }

        ushort messageId = GetMessageId(messageHeader);

        if (packetDirection == NetworkPacketDirection.Outgoing
            && !sequenceReader.TryReadBigEndian(out int sequenceId))
        {
            rawMessage = default;
            return false;
        }

        if (!ReadMessageLength(ref sequenceReader, messageHeader, out int messageLength))
        {
            rawMessage = default;
            return false;
        }

        if (!sequenceReader.TryReadExact(messageLength, out var messageContent))
        {
            rawMessage = default;
            return false;
        }

        rawMessage = new RawNetworkMessage(messageId, messageContent);
        return true;
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

        if (!sequenceReader.TryReadExact(lengthSize, out var lengthSequence))
        {
            length = 0;
            return false;
        }

        Span<byte> lengthBytes = stackalloc byte[sizeof(int)];
        lengthSequence.CopyTo(lengthBytes[^lengthSize..]);
        length = BinaryPrimitives.ReadInt32BigEndian(lengthBytes);
        return true;
    }

    private void HandleMessage(RawNetworkMessage rawMessage, NetworkPacketDirection packetDirection)
    {
        if (!NetworkMessageRegistry.TryGetTypeFromId(rawMessage.Id, out Type? messageType))
        {
            Logger.LogDebug("{0} unknown message of {1} bytes with id '{2}'",
                packetDirection == NetworkPacketDirection.Incoming ? "Received" : "Sent",
                rawMessage.Content.Length, rawMessage.Id);
            return;
        }

        var message = (INetworkMessage)Activator.CreateInstance(messageType)!;
        try
        {
            byte[] rawMessageBytes = rawMessage.Content.ToArray(); // Sad but better than recoding BinaryReader.
            using var stream = new MemoryStream(rawMessageBytes);
            using var reader = new DofusBinaryReader(stream);
            message.Deserialize(reader);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "An error occured deserializing message {0}", messageType.Name);
            return;
        }
        Logger.LogDebug("{0} message {1}",
            packetDirection == NetworkPacketDirection.Incoming ? "Received" : "Sent",
            message);

        bool written = _messagesChannel.Writer.TryWrite(message);
        Debug.Assert(written);
    }

    private record struct RawNetworkMessage(ushort Id, ReadOnlySequence<byte> Content);

    private enum NetworkPacketDirection { Incoming, Outgoing }
}