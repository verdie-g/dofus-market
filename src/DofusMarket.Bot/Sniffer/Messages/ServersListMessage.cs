using DofusMarket.Bot.Serialization;
using DofusMarket.Bot.Sniffer.Types;

namespace DofusMarket.Bot.Sniffer.Messages;

internal record ServerListMessage : INetworkMessage
{
    internal static ushort MessageId => 8557;

    public GameServerInformation[] GameServerInformation { get; private set; } = Array.Empty<GameServerInformation>();
    public bool CanCreateNewCharacter { get; private set; }

    public void Deserialize(DofusBinaryReader reader)
    {
        GameServerInformation = reader.ReadObjectCollection<GameServerInformation>();
        CanCreateNewCharacter = reader.ReadBoolean();
    }
}