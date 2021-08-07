using Dofus.Serialization;

namespace Dofus.Messages
{
    public class HelloGameMessage : INetworkMessage
    {
        internal static int MessageId => 5556;

        public void Serialize(DofusBinaryWriter writer)
        {
        }

        public void Deserialize(DofusBinaryReader reader)
        {
        }
    }
}
