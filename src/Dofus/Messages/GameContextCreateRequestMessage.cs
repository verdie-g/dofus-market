using Dofus.Serialization;

namespace Dofus.Messages
{
    public class GameContextCreateRequestMessage : INetworkMessage
    {
        internal static int MessageId => 8231;

        public void Serialize(DofusBinaryWriter writer)
        {
        }

        public void Deserialize(DofusBinaryReader reader)
        {
        }
    }
}
