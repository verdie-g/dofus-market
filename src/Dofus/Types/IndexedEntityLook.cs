using Dofus.Messages;
using Dofus.Serialization;

namespace Dofus.Types
{
    public class IndexedEntityLook : INetworkMessage
    {
        internal static int MessageId => 8160;

        public EntityLook Look { get; private set; } = new();
        public byte Index { get; private set; }

        public void Serialize(DofusBinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public void Deserialize(DofusBinaryReader reader)
        {
            Look = reader.ReadObject<EntityLook>();
            Index = reader.ReadByte();
        }
    }
}
