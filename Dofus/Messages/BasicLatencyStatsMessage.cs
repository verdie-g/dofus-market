using Dofus.Serialization;

namespace Dofus.Messages
{
    public class BasicLatencyStatsMessage : INetworkMessage
    {
        internal static int MessageId => 8738;

        public short Latency { get; set; }
        public short SampleCount { get; set; }
        public short Max { get; set; }

        public void Serialize(DofusBinaryWriter writer)
        {
            writer.Write(Latency);
            writer.Write7BitEncodedInt(SampleCount);
            writer.Write7BitEncodedInt(Max);
            writer.Write(new byte[48]); // ???
        }

        public void Deserialize(DofusBinaryReader reader)
        {
            Latency = reader.ReadInt16();
            SampleCount = (short)reader.Read7BitEncodedInt();
            Max = (short)reader.Read7BitEncodedInt();
        }
    }
}
