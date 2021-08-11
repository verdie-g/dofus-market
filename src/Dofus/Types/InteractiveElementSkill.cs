using Dofus.Messages;
using Dofus.Serialization;

namespace Dofus.Types
{
    public class InteractiveElementSkill : INetworkMessage
    {
        internal static ushort MessageId => 4345;

        public uint SkillId { get; private set; }
        public uint SkillInstanceUid { get; private set; }

        public void Serialize(DofusBinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public void Deserialize(DofusBinaryReader reader)
        {
            SkillId = (uint)reader.Read7BitEncodedInt();
            SkillInstanceUid = reader.ReadUInt32();
        }
    }
}
