using System;
using Dofus.Messages;
using Dofus.Serialization;

namespace Dofus.Types
{
    public class FightCommonInformations : INetworkMessage
    {
        internal static int MessageId => 75;

        public ushort FightId { get; private set; }
        public FightType FightType { get; private set; }

        public FightTeamInformations[] FightTeams { get; private set; } = Array.Empty<FightTeamInformations>();
        public ushort[] FightTeamsPositions { get; private set; } = Array.Empty<ushort>();
        public FightOptionsInformations[] FightTeamsOptions { get; private set; } = Array.Empty<FightOptionsInformations>();

        public void Serialize(DofusBinaryWriter writer)
        {
            throw new NotImplementedException();
        }

        public void Deserialize(DofusBinaryReader reader)
        {
            FightId = (ushort)reader.Read7BitEncodedInt();
            FightType = (FightType)reader.ReadByte();
            FightTeams = reader.ReadObjectCollection<FightTeamInformations>(true);
            FightTeamsPositions = reader.ReadCollection(r => (ushort)reader.Read7BitEncodedInt());
            FightTeamsOptions = reader.ReadObjectCollection<FightOptionsInformations>();
        }
    }
}
