using Dofus.Messages;
using Dofus.Serialization;

namespace Dofus.Types
{
    public class GameRolePlayActorInformations : GameContextActorInformations, INetworkMessage
    {
        internal new static int MessageId => 5720;

        public new void Serialize(DofusBinaryWriter writer)
        {
            base.Serialize(writer);
        }

        public new void Deserialize(DofusBinaryReader reader)
        {
            base.Deserialize(reader);
        }
    }
}
