using Dofus.Messages;
using Dofus.Serialization;

namespace Dofus.Types
{
    public class ObjectEffect : INetworkMessage
    {
        internal static int MessageId => 2671;

        public ushort ActionId { get; private set; }

        public void Serialize(DofusBinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public void Deserialize(DofusBinaryReader reader)
        {
            ActionId = (ushort)reader.Read7BitEncodedInt();
        }
    }
}
