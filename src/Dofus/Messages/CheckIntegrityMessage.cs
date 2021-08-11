using System;
using Dofus.Serialization;

namespace Dofus.Messages
{
    public class CheckIntegrityMessage : INetworkMessage
    {
        internal static ushort MessageId => 5541;
        public byte[] Data { get; init; } = Array.Empty<byte>();

        public void Serialize(DofusBinaryWriter writer)
        {
            writer.Write7BitEncodedInt(Data.Length);
            writer.Write(Data);
        }

        public void Deserialize(DofusBinaryReader reader)
        {
            throw new NotImplementedException();
        }
    }
}
