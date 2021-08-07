using Dofus.Serialization;

namespace Dofus.Messages
{
    public class InteractiveUseErrorMessage : INetworkMessage
    {
        internal static int MessageId => 5333;

        public uint ElemId { get; private set; }
        public uint SkillInstanceUid { get; private set; }

        public void Serialize(DofusBinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public void Deserialize(DofusBinaryReader reader)
        {
            ElemId = (uint)reader.Read7BitEncodedInt();
            SkillInstanceUid = (uint)reader.Read7BitEncodedInt();
        }
    }
}
