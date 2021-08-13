using Dofus.Messages;
using Dofus.Serialization;

namespace Dofus.Types
{
    public class GameRolePlayCharacterInformations : GameRolePlayHumanoidInformations, INetworkMessage
    {
        internal new static ushort MessageId => 8148;

        public ActorAlignmentInformations AlignmentInfos { get; private set; } = new();

        public new void Serialize(DofusBinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public new void Deserialize(DofusBinaryReader reader)
        {
            base.Deserialize(reader);
            AlignmentInfos = reader.ReadObject<ActorAlignmentInformations>();
        }
    }
}
