using DofusMarket.Bot.Serialization;
using DofusMarket.Bot.Sniffer.Types;

namespace DofusMarket.Bot.Sniffer.Messages;

internal record MapComplementaryInformationsDataMessage : INetworkMessage
{
    internal static ushort MessageId => 7827;

    public uint SubAreaId { get; private set; }
    public long MapId { get; private set; }

    public void Deserialize(DofusBinaryReader reader)
    {
        SubAreaId = (uint)reader.Read7BitEncodedInt();
        MapId = reader.ReadInt64();
        // Don't care about the rest.
    }
}