using Dofus.Serialization;

namespace Dofus.Messages
{
    public class ServerSelectionMessage : INetworkMessage
    {
        internal static int MessageId => 6327;

        public short ServerId { get; init; }

        public void Serialize(DofusBinaryWriter writer)
        {
            writer.Write7BitEncodedInt(ServerId);
        }

        public void Deserialize(DofusBinaryReader reader)
        {
            throw new System.NotImplementedException();
        }
    }
}
