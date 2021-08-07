using Dofus.Serialization;

namespace Dofus.Messages
{
    public class ChatAbstractServerMessage : INetworkMessage
    {
        internal static int MessageId => 7258;

        public byte Channel { get; private set; }
        public string Content { get; private set; } = string.Empty;
        public int TimeStamp { get; private set; }
        public string Fingerprint { get; private set; } = string.Empty;

        public void Serialize(DofusBinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public void Deserialize(DofusBinaryReader reader)
        {
            Channel = reader.ReadByte();
            Content = reader.ReadString();
            TimeStamp = reader.ReadInt32();
            Fingerprint = reader.ReadString();
        }
    }
}
