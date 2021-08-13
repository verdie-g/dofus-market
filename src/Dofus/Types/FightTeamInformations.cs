using System;
using Dofus.Messages;
using Dofus.Serialization;

namespace Dofus.Types
{
    public class FightTeamInformations : AbstractFightTeamInformations, INetworkMessage
    {
        internal new static ushort MessageId => 4316;

        public FightTeamMemberInformations[] TeamMembers { get; private set; } = Array.Empty<FightTeamMemberInformations>();

        public new void Serialize(DofusBinaryWriter writer)
        {
            throw new NotImplementedException();
        }

        public new void Deserialize(DofusBinaryReader reader)
        {
            base.Deserialize(reader);
            TeamMembers = reader.ReadObjectCollection<FightTeamMemberInformations>(true);
        }
    }
}
