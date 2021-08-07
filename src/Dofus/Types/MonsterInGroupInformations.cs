using Dofus.Messages;
using Dofus.Serialization;

namespace Dofus.Types
{
    public class MonsterInGroupInformations : MonsterInGroupLightInformations, INetworkMessage
    {
        internal new static int MessageId => 3872;

        public EntityLook Look { get; private set; } = new();

        public new void Serialize(DofusBinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public new void Deserialize(DofusBinaryReader reader)
        {
            base.Deserialize(reader);
            Look = reader.ReadObject<EntityLook>();
        }
    }
}
