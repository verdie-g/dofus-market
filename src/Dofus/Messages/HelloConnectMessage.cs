using System;
using Dofus.Serialization;

namespace Dofus.Messages
{
    public class HelloConnectMessage : INetworkMessage
    {
        internal static int MessageId => 1030;
        public string Salt { get; private set; } = string.Empty;
        public byte[] Key { get; private set; } = Array.Empty<byte>();

        public void Serialize(DofusBinaryWriter writer) => throw new NotImplementedException();

        public void Deserialize(DofusBinaryReader reader)
        {
            Salt = reader.ReadString();
            int keyLength = reader.Read7BitEncodedInt();
            Key = reader.ReadBytes(keyLength);
        }
    }
}
