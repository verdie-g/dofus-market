using DofusMarket.Bot.Serialization;
using DofusMarket.Bot.Sniffer.Types;

namespace DofusMarket.Bot.Sniffer.Messages;

internal record CharactersListMessage : INetworkMessage
{
    internal static ushort MessageId => 1165;

    public void Deserialize(DofusBinaryReader reader)
    {
        // TODO: the content doesn't really matter
    }
}