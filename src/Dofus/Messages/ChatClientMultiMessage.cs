using Dofus.Serialization;

namespace Dofus.Messages
{
    public class ChatClientMultiMessage : ChatAbstractClientMessage, INetworkMessage
    {
        internal new static ushort MessageId => 107;

        public byte Channel { get; set; }

        public new void Serialize(DofusBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(Channel);
        }

        public new void Deserialize(DofusBinaryReader reader)
        {
            throw new System.NotImplementedException();
        }
    }
}
