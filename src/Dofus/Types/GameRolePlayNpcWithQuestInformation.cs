using Dofus.Messages;
using Dofus.Serialization;

namespace Dofus.Types
{
    public class GameRolePlayNpcWithQuestInformation : GameRolePlayNpcInformations, INetworkMessage
    {
        internal new static ushort MessageId => 7842;

        public GameRolePlayNpcQuestFlag QuestFlag { get; private set; } = new();

        public new void Serialize(DofusBinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public new void Deserialize(DofusBinaryReader reader)
        {
            base.Deserialize(reader);
            QuestFlag = reader.ReadObject<GameRolePlayNpcQuestFlag>();
        }
    }
}
