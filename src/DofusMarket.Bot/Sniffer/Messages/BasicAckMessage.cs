using DofusMarket.Bot.Serialization;

namespace DofusMarket.Bot.Sniffer.Messages;

internal record BasicAckMessage : INetworkMessage
{
    internal static ushort MessageId => 9898;

    public void Deserialize(DofusBinaryReader reader)
    {
        // doesn't matter
    }
}