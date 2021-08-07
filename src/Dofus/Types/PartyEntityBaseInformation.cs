using Dofus.Messages;
using Dofus.Serialization;

namespace Dofus.Types
{
    public class PartyEntityBaseInformation : INetworkMessage
    {
        internal static int MessageId => 8446;

        public byte IndexId { get; private set; }
        public byte EntityModelId { get; private set; }
        public EntityLook EntityLook { get; private set; } = new();

        public void Serialize(DofusBinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public void Deserialize(DofusBinaryReader reader)
        {
            IndexId = reader.ReadByte();
            EntityModelId = reader.ReadByte();
            EntityLook = reader.ReadObject<EntityLook>();
        }
    }
}
