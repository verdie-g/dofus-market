using Dofus.Messages;
using Dofus.Serialization;

namespace Dofus.Types
{
    public class CharacterBasicMinimalInformations : AbstractCharacterInformation, INetworkMessage
    {
        internal new static int MessageId => 6835;

        public string Name { get; private set; } = string.Empty;

        public new void Serialize(DofusBinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public new void Deserialize(DofusBinaryReader reader)
        {
            base.Deserialize(reader);
            Name = reader.ReadString();
        }
    }
}
