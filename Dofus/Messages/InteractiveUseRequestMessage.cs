using Dofus.Serialization;

namespace Dofus.Messages
{
    public class InteractiveUseRequestMessage : INetworkMessage
    {
        internal static int MessageId => 7117;

        public uint ElemId { get; init; }
        public uint SkillInstanceUid { get; init; }

        public void Serialize(DofusBinaryWriter writer)
        {
            writer.Write7BitEncodedInt((int)ElemId);
            writer.Write7BitEncodedInt((int)SkillInstanceUid);
        }

        public void Deserialize(DofusBinaryReader reader)
        {
            throw new System.NotImplementedException();
        }
    }
}
