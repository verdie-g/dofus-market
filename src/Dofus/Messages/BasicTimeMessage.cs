using Dofus.Serialization;

namespace Dofus.Messages
{
    public class BasicTimeMessage : INetworkMessage
    {
        internal static int MessageId => 4907;

        public double TimeStamp { get; private set; }
        public short TimeZoneOffset { get; private set; }

        public void Serialize(DofusBinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public void Deserialize(DofusBinaryReader reader)
        {
            TimeStamp = reader.ReadDouble();
            TimeZoneOffset = reader.ReadInt16();
        }
    }
}
