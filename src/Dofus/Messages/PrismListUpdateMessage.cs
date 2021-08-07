using Dofus.Serialization;

namespace Dofus.Messages
{
    public class PrismListUpdateMessage : INetworkMessage
    {
        internal static int MessageId => 101;

        public void Serialize(DofusBinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public void Deserialize(DofusBinaryReader reader)
        {
        }
    }
}
