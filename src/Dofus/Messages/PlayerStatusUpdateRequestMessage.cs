using Dofus.Serialization;
using Dofus.Types;

namespace Dofus.Messages
{
    public class PlayerStatusUpdateRequestMessage : INetworkMessage
    {
        internal static ushort MessageId => 9100;

        public PlayerStatus Status { get; set; } = new();

        public void Serialize(DofusBinaryWriter writer)
        {
            writer.Write(NetworkMessageRegistry.GetIdFromType(Status.GetType()));
            Status.Serialize(writer);
        }

        public void Deserialize(DofusBinaryReader reader)
        {
            throw new System.NotImplementedException();
        }
    }
}
