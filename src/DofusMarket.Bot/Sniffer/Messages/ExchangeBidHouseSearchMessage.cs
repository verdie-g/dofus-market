using DofusMarket.Bot.Serialization;

namespace DofusMarket.Bot.Sniffer.Messages;

internal record ExchangeBidHouseSearchMessage : INetworkMessage
{
    internal static ushort MessageId => 2056;

    public ushort ObjectGid { get; set; }
    public bool Follow { get; set; }

    public void Deserialize(DofusBinaryReader reader)
    {
        ObjectGid = (ushort)reader.Read7BitEncodedInt();
        Follow = reader.ReadBoolean();
    }
}