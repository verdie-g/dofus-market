using Dofus.Serialization;

namespace Dofus.Messages
{
    public class LoginQueueStatusMessage : INetworkMessage
    {
        internal static int MessageId => 2553;
        public ushort Position { get; private set; }
        public ushort Total { get; private set; }

        public void Serialize(DofusBinaryWriter writer)
        {
            writer.Write(Position);
            writer.Write(Total);
        }

        public void Deserialize(DofusBinaryReader reader)
        {
            Position = reader.ReadUInt16();
            Total = reader.ReadUInt16();
        }
    }
}
