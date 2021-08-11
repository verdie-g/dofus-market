using Dofus.Serialization;
using Dofus.Types;

namespace Dofus.Messages
{
    public class GameFightUpdateTeamMessage : INetworkMessage
    {
        internal static ushort MessageId => 6859;

        public ushort FightId { get; private set; }
        public FightTeamInformations Team { get; private set; } = new();

        public void Serialize(DofusBinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public void Deserialize(DofusBinaryReader reader)
        {
            FightId = (ushort)reader.Read7BitEncodedInt();
            Team = reader.ReadObject<FightTeamInformations>();
        }
    }
}
