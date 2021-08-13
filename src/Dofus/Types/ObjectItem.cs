using System;
using Dofus.Messages;
using Dofus.Serialization;

namespace Dofus.Types
{
    public class ObjectItem : Item, INetworkMessage
    {
        internal new static ushort MessageId => 224;

        public short Position { get; private set; }
        public ushort ObjectGid { get; private set; }
        public ObjectEffect[] Effects { get; private set; } = Array.Empty<ObjectEffect>();
        public uint ObjectUid { get; private set; }
        public uint Quantity { get; private set; }

        public new void Serialize(DofusBinaryWriter writer)
        {
            throw new NotImplementedException();
        }

        public new void Deserialize(DofusBinaryReader reader)
        {
            base.Deserialize(reader);
            Position = reader.ReadInt16();
            ObjectGid = (ushort)reader.Read7BitEncodedInt();
            Effects = reader.ReadObjectCollection<ObjectEffect>(true);
            ObjectUid = (uint)reader.Read7BitEncodedInt();
            Quantity = (uint)reader.Read7BitEncodedInt();
        }
    }
}
