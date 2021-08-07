using Dofus.Messages;
using Dofus.Serialization;

namespace Dofus.Types
{
    public class FightTeamMemberTaxCollectorInformations : FightTeamMemberInformations, INetworkMessage
    {
        internal new static int MessageId => 9221;

        public ushort FirstNameId { get; private set; }
        public ushort LastNameId { get; private set; }
        public byte Level { get; private set; }
        public uint GuildId { get; private set; }
        public long Uid { get; private set; }

        public new void Serialize(DofusBinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public new void Deserialize(DofusBinaryReader reader)
        {
            base.Deserialize(reader);
            FirstNameId = (ushort)reader.Read7BitEncodedInt();
            LastNameId = (ushort)reader.Read7BitEncodedInt();
            Level = reader.ReadByte();
            GuildId = (uint)reader.Read7BitEncodedInt();
            Uid = reader.ReadInt64();
        }
    }
}
