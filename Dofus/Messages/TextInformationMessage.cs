using System;
using Dofus.Serialization;
using Dofus.Types;

namespace Dofus.Messages
{
    public class TextInformationMessage : INetworkMessage
    {
        internal static int MessageId => 8845;

        public TextInformationType MsgType { get; private set; }
        public ushort MsgId { get; private set; }
        public string[] Parameters { get; private set; } = Array.Empty<string>();

        public void Serialize(DofusBinaryWriter writer)
        {
            throw new NotImplementedException();
        }

        public void Deserialize(DofusBinaryReader reader)
        {
            MsgType = (TextInformationType)reader.ReadByte();
            MsgId = (ushort)reader.Read7BitEncodedInt();
            Parameters = reader.ReadCollection(r => r.ReadString());
        }
    }
}
