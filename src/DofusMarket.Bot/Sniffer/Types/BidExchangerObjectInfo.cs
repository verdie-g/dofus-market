using DofusMarket.Bot.Serialization;

namespace DofusMarket.Bot.Sniffer.Types;

internal record BidExchangerObjectInfo : INetworkMessage
{
    internal static ushort MessageId => 54;

    public uint ObjectUid { get; private set; }
    public ushort ObjectGid { get; private set; }
    public uint ObjectType { get; private set; }
    public ObjectEffect[] Effects { get; private set; } = Array.Empty<ObjectEffect>();
    public ulong[] Prices { get; private set; } = Array.Empty<ulong>();

    public void Deserialize(DofusBinaryReader reader)
    {
        ObjectUid = (uint)reader.Read7BitEncodedInt();
        ObjectGid = (ushort)reader.Read7BitEncodedInt();
        ObjectType = reader.ReadUInt32();
        Effects = reader.ReadObjectCollection<ObjectEffect>(true);
        Prices = reader.ReadCollection(r => (ulong)reader.Read7BitEncodedInt64());
    }
}