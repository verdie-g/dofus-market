using System;
using Dofus.Serialization;

namespace Dofus.Messages
{
    public class UpdateMapPlayersAgressableStatusMessage : INetworkMessage
    {
        internal static ushort MessageId => 8171;

        public ulong[] PlayerIds { get; private set; } = Array.Empty<ulong>();
        public byte[] Enable { get; private set; } = Array.Empty<byte>();

        public void Serialize(DofusBinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public void Deserialize(DofusBinaryReader reader)
        {
            PlayerIds = reader.ReadCollection(r => (ulong)r.Read7BitEncodedInt64());
            Enable = reader.ReadCollection(r => r.ReadByte());
        }
    }
}
