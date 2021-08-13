using Dofus.Messages;
using Dofus.Serialization;

namespace Dofus.Types
{
    public class GameRolePlayHumanoidInformations : GameRolePlayNamedActorInformations, INetworkMessage
    {
        internal new static ushort MessageId => 4263;

        public HumanInformations HumanoidInfo { get; private set; } = new();
        public int AccountId { get; private set; }

        public new void Serialize(DofusBinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public new void Deserialize(DofusBinaryReader reader)
        {
            base.Deserialize(reader);
            HumanoidInfo = reader.ReadObject<HumanInformations>(true);
            AccountId = reader.ReadInt32();
        }
    }
}
