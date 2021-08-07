using System;
using Dofus.Messages;
using Dofus.Serialization;

namespace Dofus.Types
{
    public class BidExchangerObjectInfo : INetworkMessage
    {
        internal static int MessageId => 8892;

        public uint ObjectUid { get; private set; }
        public ushort ObjectGid { get; private set; }
        public int ObjectType { get; private set; }
        public ObjectEffect[] Effects { get; private set; } = Array.Empty<ObjectEffect>();
        public ulong[] Prices { get; private set; } = Array.Empty<ulong>();

        public void Serialize(DofusBinaryWriter writer)
        {
            throw new NotImplementedException();
        }

        public void Deserialize(DofusBinaryReader reader)
        {
            ObjectUid = (uint)reader.Read7BitEncodedInt();
            ObjectGid = (ushort)reader.Read7BitEncodedInt();
            ObjectType = reader.ReadInt32();
            Effects = reader.ReadObjectCollection<ObjectEffect>(true);
            Prices = reader.ReadCollection(r => (ulong)reader.Read7BitEncodedInt64());
        }
    }
}
