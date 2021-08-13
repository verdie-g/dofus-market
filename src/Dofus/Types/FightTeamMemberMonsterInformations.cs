using Dofus.Messages;
using Dofus.Serialization;

namespace Dofus.Types
{
    public class FightTeamMemberMonsterInformations : FightTeamMemberInformations, INetworkMessage
    {
        internal new static ushort MessageId => 8655;

        public int MonsterId { get; private set; }
        public byte Grade { get; private set; }

        public new void Serialize(DofusBinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public new void Deserialize(DofusBinaryReader reader)
        {
            base.Deserialize(reader);
            MonsterId = reader.ReadInt32();
            Grade = reader.ReadByte();
        }
    }
}
