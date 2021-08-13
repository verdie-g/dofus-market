using Dofus.Messages;
using Dofus.Serialization;

namespace Dofus.Types
{
    public class HumanOptionOrnament : HumanOption, INetworkMessage
    {
        internal new static ushort MessageId => 6314;

        public ushort OrnamentId { get; private set; }
        public ushort LevelId { get; private set; }
        public short LeagueId { get; private set; }
        public int LadderPosition { get; private set; }

        public new void Serialize(DofusBinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public new void Deserialize(DofusBinaryReader reader)
        {
            base.Deserialize(reader);
            OrnamentId = (ushort)reader.Read7BitEncodedInt();
            LevelId = (ushort)reader.Read7BitEncodedInt();
            LeagueId = (short)reader.Read7BitEncodedInt();
            LadderPosition = reader.ReadInt32();
        }
    }
}
