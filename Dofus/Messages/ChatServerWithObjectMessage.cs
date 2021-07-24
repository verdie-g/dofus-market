using System;
using Dofus.Serialization;
using Dofus.Types;

namespace Dofus.Messages
{
    public class ChatServerWithObjectMessage : ChatServerMessage, INetworkMessage
    {
        internal new static int MessageId => 5393;

        public ObjectItem[] Objects { get; private set; } = Array.Empty<ObjectItem>();

        public new void Serialize(DofusBinaryWriter writer)
        {
            throw new NotImplementedException();
        }

        public new void Deserialize(DofusBinaryReader reader)
        {
            base.Deserialize(reader);
            Objects = reader.ReadObjectCollection<ObjectItem>();
        }
    }
}
