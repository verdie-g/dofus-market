using System;
using Dofus.Serialization;

namespace Dofus.Messages
{
    public class ProtocolRequiredMessage : INetworkMessage
    {
        internal static ushort MessageId => 9546;
        public string Version { get; private set; } = string.Empty;

        public void Serialize(DofusBinaryWriter writer) => throw new NotImplementedException();

        public void Deserialize(DofusBinaryReader reader)
        {
            Version = reader.ReadString();
        }
    }
}
