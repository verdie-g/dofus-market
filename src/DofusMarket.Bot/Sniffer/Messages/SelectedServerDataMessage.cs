using DofusMarket.Bot.Serialization;

namespace DofusMarket.Bot.Sniffer.Messages;

internal record SelectedServerDataMessage : INetworkMessage
{
    internal static ushort MessageId => 2882;

    public uint ServerId { get; private set; }
    public string Address { get; private set; } = string.Empty;
    public uint[] Ports { get; private set; } = Array.Empty<uint>();
    public bool CanCreateNewCharacter { get; private set; }
    public byte[] Ticket { get; private set; } = Array.Empty<byte>();

    public void Deserialize(DofusBinaryReader reader)
    {
        ServerId = (uint)reader.Read7BitEncodedInt();
        Address = reader.ReadString();
        Ports = reader.ReadCollection(r => (uint)r.Read7BitEncodedInt());
        CanCreateNewCharacter = reader.ReadBoolean();
        int ticketLength = reader.Read7BitEncodedInt();
        Ticket = reader.ReadBytes(ticketLength);
    }
}