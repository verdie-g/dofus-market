using DofusMarket.Bot.Serialization;

namespace DofusMarket.Bot.Sniffer.Messages;

internal record ExchangeTypesExchangerDescriptionForUserMessage : INetworkMessage
{
    internal static ushort MessageId => 4903;

    public uint ObjectType { get; private set; }
    public uint[] TypeDescription { get; private set; } = Array.Empty<uint>();

    public void Deserialize(DofusBinaryReader reader)
    {
        ObjectType = reader.ReadUInt32();
        TypeDescription = reader.ReadCollection(r => (uint)r.Read7BitEncodedInt());
    }
}