using Dofus.Messages;
using Dofus.Serialization;

namespace Dofus.Types
{
    public class GameRolePlayNamedActorInformations : GameRolePlayActorInformations, INetworkMessage
    {
        internal new static int MessageId => 4623;

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
