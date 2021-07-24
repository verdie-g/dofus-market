using Dofus.Messages;
using Dofus.Serialization;

namespace Dofus.Types
{
    public class FightTeamMemberWithAllianceCharacterInformations : FightTeamMemberCharacterInformations, INetworkMessage
    {
        internal new static int MessageId => 3733;

        public BasicAllianceInformations AllianceInfos { get; set; } = new();

        public new void Serialize(DofusBinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public new void Deserialize(DofusBinaryReader reader)
        {
            base.Deserialize(reader);
            AllianceInfos = reader.ReadObject<BasicAllianceInformations>();
        }
    }
}
