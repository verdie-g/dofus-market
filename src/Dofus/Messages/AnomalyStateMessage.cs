using Dofus.Serialization;

namespace Dofus.Messages
{
    public class AnomalyStateMessage : INetworkMessage
    {
        internal static int MessageId => 1170;

        public ushort SubareaId { get; private set; }
        public bool Open { get; private set; }
        public ulong ClosingTime { get; private set; }

        public void Serialize(DofusBinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public void Deserialize(DofusBinaryReader reader)
        {
            SubareaId = (ushort)reader.Read7BitEncodedInt();
            Open = reader.ReadBoolean();
            ClosingTime = (ulong)reader.Read7BitEncodedInt64();
        }
    }
}
