using Dofus.Serialization;

namespace Dofus.Messages
{
    public class ChatAbstractClientMessage : INetworkMessage
    {
        internal static ushort MessageId => 4914;

        public string Content { get; set; } = string.Empty;

        public void Serialize(DofusBinaryWriter writer)
        {
            writer.Write(Content);
        }

        public void Deserialize(DofusBinaryReader reader)
        {
            throw new System.NotImplementedException();
        }
    }
}
