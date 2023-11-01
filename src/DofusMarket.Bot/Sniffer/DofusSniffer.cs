using System.Buffers;
using System.Buffers.Binary;
using System.Diagnostics;
using System.IO.Pipelines;
using System.Net;
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
    private readonly Pipe _tcpPipe;
    private readonly CancellationTokenSource _tcpPipeCancellation;
    private readonly Channel<INetworkMessage> _messagesChannel;
    private ILiveDevice? _device;

    public DofusSniffer(string networkDeviceId)
    {
        _networkDeviceId = networkDeviceId;
        _tcpPipe = new Pipe();
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

        Task.Run(() => FillPipeAsync(_tcpPipeCancellation.Token));
        Task.Run(() => ReadPipeAsync(_tcpPipeCancellation.Token));

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

    private async Task FillPipeAsync(CancellationToken cancellationToken)
    {
        Debug.Assert(_device != null);

        var loginServerIpAddresses = await Dns.GetHostAddressesAsync(DofusLoginServerHostName, cancellationToken);

        _device.Open();
        _device.Filter = $"tcp and (src port {DofusServersPort}) and (net {DofusGameServersIpRange} or host {string.Join<IPAddress>(" or host ", loginServerIpAddresses)})";

        Logger.LogInformation("Starting the sniffer filtering on \"{0}\"", _device.Filter);

        var pipeWriter = _tcpPipe.Writer;
        GetPacketStatus packetStatus;
        do
        {
            packetStatus = GetNextTcpPacketPayload(out byte[] tcpPayload);
            if (packetStatus == GetPacketStatus.PacketRead)
            {
                await pipeWriter.WriteAsync(tcpPayload, cancellationToken);
            }
        } while (packetStatus is GetPacketStatus.PacketRead or GetPacketStatus.ReadTimeout);

        Logger.LogError("Error reading packet. Shutting down the sniffer");

        await pipeWriter.CompleteAsync();
    }

    private GetPacketStatus GetNextTcpPacketPayload(out byte[] tcpPayload)
    {
        Debug.Assert(_device != null);

        var packetStatus = _device.GetNextPacket(out PacketCapture e);
        if (packetStatus != GetPacketStatus.PacketRead)
        {
            tcpPayload = Array.Empty<byte>();
            return packetStatus;
        }

        var rawPacket = e.GetPacket();
        var packet = Packet.ParsePacket(rawPacket.LinkLayerType, rawPacket.Data);

        var tcpPacket = packet.Extract<TcpPacket>();
        tcpPayload = tcpPacket.PayloadData;
        return packetStatus;
    }

    private async Task ReadPipeAsync(CancellationToken cancellationToken)
    {
        var pipeReader = _tcpPipe.Reader;
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

    private void HandleMessage(RawNetworkMessage rawMessage)
    {
        if (!NetworkMessageRegistry.TryGetTypeFromId(rawMessage.Id, out Type? messageType))
        {
            Logger.LogDebug("Ignoring unknown message of {0} bytes with id '{1}'", rawMessage.Content.Length, rawMessage.Id);
            return;
        }

        Logger.LogDebug("Received message {0}", messageType.Name);
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

        bool written = _messagesChannel.Writer.TryWrite(message);
        Debug.Assert(written);
    }

    private record struct RawNetworkMessage(ushort Id, ReadOnlySequence<byte> Content);
}