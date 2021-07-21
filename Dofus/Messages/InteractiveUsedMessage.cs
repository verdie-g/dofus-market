using Dofus.Serialization;

namespace Dofus.Messages
{
    public class InteractiveUsedMessage : INetworkMessage
    {
        internal static int MessageId => 9664;

        public ulong EntityId { get; private set; }
        public uint ElemId { get; private set; }
        public ushort SkillId { get; private set; }
        public ushort Duration { get; private set; }
        public bool CanMove { get; private set; }

        public void Serialize(DofusBinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public void Deserialize(DofusBinaryReader reader)
        {
            EntityId = (ulong)reader.Read7BitEncodedInt64();
            ElemId = (uint)reader.Read7BitEncodedInt();
            SkillId = (ushort)reader.Read7BitEncodedInt();
            Duration = (ushort)reader.Read7BitEncodedInt();
            CanMove = reader.ReadBoolean();
        }
    }
}
