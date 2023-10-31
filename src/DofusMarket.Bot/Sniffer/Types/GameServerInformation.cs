using DofusMarket.Bot.Serialization;

namespace DofusMarket.Bot.Sniffer.Types;

internal record GameServerInformation : INetworkMessage
{
    internal static ushort MessageId => 8807;

    public bool IsMonoAccount { get; private set; }
    public bool IsSelectable { get; private set; }
    public uint Id { get; private set; }
    public byte Type { get; private set; }
    public byte Status { get; private set; }
    public byte Completion { get; private set; }
    public byte CharactersCount { get; private set; }
    public byte CharactersSlot { get; private set; }
    public long Date { get; private set; }

    public void Deserialize(DofusBinaryReader reader)
    {
        byte flags = reader.ReadByte();
        IsMonoAccount = (flags & 0x1) != 0;
        IsSelectable = (flags & 0x2) != 0;
        Id = (uint)reader.Read7BitEncodedInt();
        Type = reader.ReadByte();
        Status = reader.ReadByte();
        Completion = reader.ReadByte();
        CharactersCount = reader.ReadByte();
        CharactersSlot = reader.ReadByte();
        Date = reader.ReadInt64();
    }
}