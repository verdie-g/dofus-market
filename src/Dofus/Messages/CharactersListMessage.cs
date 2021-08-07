using Dofus.Serialization;

namespace Dofus.Messages
{
    public class CharactersListMessage : BasicCharacterListMessage, INetworkMessage
    {
        internal new static int MessageId => 7692;

        public bool HasStartupActions { get; private set; }

        public new void Serialize(DofusBinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public new void Deserialize(DofusBinaryReader reader)
        {
            base.Deserialize(reader);
            HasStartupActions = reader.ReadBoolean();
        }
    }
}
