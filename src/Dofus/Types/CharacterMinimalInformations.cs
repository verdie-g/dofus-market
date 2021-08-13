using Dofus.Messages;
using Dofus.Serialization;

namespace Dofus.Types
{
    public class CharacterMinimalInformations : CharacterBasicMinimalInformations, INetworkMessage
    {
        internal new static ushort MessageId => 2126;

        public ushort Level { get; private set; }

        public new void Serialize(DofusBinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public new void Deserialize(DofusBinaryReader reader)
        {
            base.Deserialize(reader);
            Level = (ushort)reader.Read7BitEncodedInt();
        }
    }
}
