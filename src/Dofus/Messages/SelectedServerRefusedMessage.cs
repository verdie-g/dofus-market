using Dofus.Serialization;
using Dofus.Types;

namespace Dofus.Messages
{
    public class SelectedServerRefusedMessage : INetworkMessage
    {
        internal static int MessageId => 506;

        public ushort ServerId { get; private set; }
        public ServerConnectionError Error { get; private set; }
        public ServerStatus ServerStatus { get; private set; }

        public void Serialize(DofusBinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public void Deserialize(DofusBinaryReader reader)
        {
            ServerId = (ushort)reader.Read7BitEncodedInt();
            Error = (ServerConnectionError)reader.ReadByte();
            ServerStatus = (ServerStatus)reader.ReadByte();
        }
    }
}
