using Dofus.Messages;
using Dofus.Serialization;

namespace Dofus.Types
{
    public class CharacterMinimalPlusLookInformations : CharacterMinimalInformations, INetworkMessage
    {
        internal new static int MessageId => 1956;

        public EntityLook EntityLook { get; private set; } = new();
        public Breed Breed { get; private set; }

        public new void Serialize(DofusBinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public new void Deserialize(DofusBinaryReader reader)
        {
            base.Deserialize(reader);
            EntityLook.Deserialize(reader);
            Breed = (Breed)reader.ReadByte();
        }
    }
}
