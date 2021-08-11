using System;
using Dofus.Serialization;

namespace Dofus.Messages
{
    public class SystemMessageDisplayMessage : INetworkMessage
    {
        internal static ushort MessageId => 1803;

        public bool HangUp { get; private set; }
        public ushort MsgId { get; private set; }
        public string[] Parameters { get; private set; } = Array.Empty<string>();

        public void Serialize(DofusBinaryWriter writer)
        {
            throw new NotImplementedException();
        }

        public void Deserialize(DofusBinaryReader reader)
        {
            HangUp = reader.ReadBoolean();
            MsgId = (ushort)reader.Read7BitEncodedInt();
            Parameters = reader.ReadCollection(r => r.ReadString());
        }
    }
}
