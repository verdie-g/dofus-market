using Dofus.Messages;
using Dofus.Serialization;

namespace Dofus.Types
{
    public class AbstractFightTeamInformations : INetworkMessage
    {
        internal static int MessageId => 2033;

        public byte TeamId { get; private set; }
        public long LeaderId { get; private set; }
        public TeamSide TeamSide { get; private set; }
        public TeamType TeamType { get; private set; }
        public byte NbWaves { get; private set; }

        public void Serialize(DofusBinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public void Deserialize(DofusBinaryReader reader)
        {
            TeamId = reader.ReadByte();
            LeaderId = reader.ReadInt64();
            TeamSide = (TeamSide)reader.ReadByte();
            TeamType = (TeamType)reader.ReadByte();
            NbWaves = reader.ReadByte();
        }
    }
}
