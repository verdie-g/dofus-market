using Dofus.Messages;
using Dofus.Serialization;

namespace Dofus.Types
{
    public class HumanOptionSkillUse : HumanOption, INetworkMessage
    {
        internal new static ushort MessageId => 1691;

        public uint ElementId { get; private set; }
        public ushort SkillId { get; private set; }
        public double SkillEndTime { get; private set; }

        public new void Serialize(DofusBinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public new void Deserialize(DofusBinaryReader reader)
        {
            base.Deserialize(reader);
            ElementId = (uint)reader.Read7BitEncodedInt();
            SkillId = (ushort)reader.Read7BitEncodedInt();
            SkillEndTime = reader.ReadDouble();
        }
    }
}
