using DofusMarket.Bot.Serialization;

namespace DofusMarket.Bot.Sniffer;

internal interface INetworkMessage
{
    void Deserialize(DofusBinaryReader reader);
}