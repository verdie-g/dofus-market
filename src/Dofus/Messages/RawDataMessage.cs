using System;
using Dofus.Serialization;

namespace Dofus.Messages
{
    public class RawDataMessage : INetworkMessage
    {
        internal static int MessageId => 124;

        public byte[] Content { get; private set; } = Array.Empty<byte>();

        public void Serialize(DofusBinaryWriter writer)
        {
            throw new NotImplementedException();
        }

        public void Deserialize(DofusBinaryReader reader)
        {
            int length = reader.Read7BitEncodedInt();
            Content = reader.ReadBytes(length);
        }
    }
}
