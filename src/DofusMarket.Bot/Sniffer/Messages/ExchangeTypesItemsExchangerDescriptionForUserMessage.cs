using DofusMarket.Bot.Serialization;
using DofusMarket.Bot.Sniffer.Types;

namespace DofusMarket.Bot.Sniffer.Messages;

internal record ExchangeTypesItemsExchangerDescriptionForUserMessage : INetworkMessage
{
    internal static ushort MessageId => 6222;

    public ushort ObjectGid { get; private set; }
    public uint ObjectType { get; private set; }
    public BidExchangerObjectInfo[] ItemTypeDescriptions { get; private set; } = Array.Empty<BidExchangerObjectInfo>();

    public void Deserialize(DofusBinaryReader reader)
    {
        ObjectGid = (ushort)reader.Read7BitEncodedInt();
        ObjectType = reader.ReadUInt32();
        ItemTypeDescriptions = reader.ReadObjectCollection<BidExchangerObjectInfo>();
    }
}