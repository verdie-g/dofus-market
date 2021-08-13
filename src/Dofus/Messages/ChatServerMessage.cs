using Dofus.Serialization;

namespace Dofus.Messages
{
    public class ChatServerMessage : ChatAbstractServerMessage, INetworkMessage
    {
        internal new static ushort MessageId => 5855;

        public long SenderId { get; private set; }
        public string SenderName { get; private set; } = string.Empty;
        public string Prefix { get; private set; } = string.Empty;
        public int SenderAccountId { get; private set; }

        public new void Serialize(DofusBinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public new void Deserialize(DofusBinaryReader reader)
        {
            base.Deserialize(reader);
            SenderId = reader.ReadInt64();
            SenderName = reader.ReadString();
            Prefix = reader.ReadString();
            SenderAccountId = reader.ReadInt32();
        }
    }
}
