using DofusMarket.Bot.Serialization;

namespace DofusMarket.Bot.Sniffer.Types;

internal record SellerBuyerDescriptor : INetworkMessage
{
    internal static ushort MessageId => 3010;

    public uint[] Quantities { get; private set; } = Array.Empty<uint>();
    public uint[] Types { get; private set; } = Array.Empty<uint>();
    public float TaxPercentage { get; private set; }
    public float TaxModificationPercentage { get; private set; }
    public byte MaxLevelItem { get; private set; }
    public uint MaxItemPerAccount { get; private set; }
    public int NpcContextualId { get; private set; }
    public ushort UnsoldDelay { get; private set; }

    public void Deserialize(DofusBinaryReader reader)
    {
        Quantities = reader.ReadCollection(r => (uint)r.Read7BitEncodedInt());
        Types = reader.ReadCollection(r => (uint)r.Read7BitEncodedInt());
        TaxPercentage = reader.ReadSingle();
        TaxModificationPercentage = reader.ReadSingle();
        MaxLevelItem = reader.ReadByte();
        MaxItemPerAccount = (uint)reader.Read7BitEncodedInt();
        NpcContextualId = reader.ReadInt32();
        UnsoldDelay = (ushort)reader.Read7BitEncodedInt();
    }
}