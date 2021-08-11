using System;
using Dofus.Serialization;
using Dofus.Types;

namespace Dofus.Messages
{
    public class ServerListMessage : INetworkMessage
    {
        internal static ushort MessageId => 4096;

        public GameServerInformations[] Servers { get; private set; } = Array.Empty<GameServerInformations>();
        public short AlreadyConnectedToServerId { get; private set; }
        public bool CanCreateNewCharacter { get; private set; }

        public void Serialize(DofusBinaryWriter writer)
        {
            throw new NotImplementedException();
        }

        public void Deserialize(DofusBinaryReader reader)
        {
            Servers = reader.ReadObjectCollection<GameServerInformations>();
            AlreadyConnectedToServerId = (short)reader.Read7BitEncodedInt();
            CanCreateNewCharacter = reader.ReadByte() == 1;
        }
    }
}
