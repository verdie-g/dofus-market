using Dofus.Messages;
using Dofus.Serialization;

namespace Dofus.Types
{
    public class CharacterBaseInformations : CharacterMinimalPlusLookInformations, INetworkMessage
    {
        internal new static ushort MessageId => 7196;

        public bool Sex { get; private set; }

        public new void Serialize(DofusBinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public new void Deserialize(DofusBinaryReader reader)
        {
            base.Deserialize(reader);
            Sex = reader.ReadBoolean();
        }
    }
}
