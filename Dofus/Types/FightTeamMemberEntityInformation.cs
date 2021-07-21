using Dofus.Messages;
using Dofus.Serialization;

namespace Dofus.Types
{
    public class FightTeamMemberEntityInformation : FightTeamMemberInformations, INetworkMessage
    {
        internal new static int MessageId => 4737;

        public byte EntityModelId { get; private set; }
        public ushort Level { get; private set; }
        public long MasterId { get; private set; }

        public new void Serialize(DofusBinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public new void Deserialize(DofusBinaryReader reader)
        {
            base.Deserialize(reader);
            EntityModelId = reader.ReadByte();
            Level = (ushort)reader.Read7BitEncodedInt();
            MasterId = reader.ReadInt64();
        }
    }
}
