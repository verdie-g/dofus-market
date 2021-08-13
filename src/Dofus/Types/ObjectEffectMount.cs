using System;
using Dofus.Messages;
using Dofus.Serialization;

namespace Dofus.Types
{
    public class ObjectEffectMount : ObjectEffect, INetworkMessage
    {
        internal new static ushort MessageId => 5871;

        public bool Sex { get; private set; }
        public bool IsRideable { get; private set; }
        public bool IsFeconded { get; private set; }
        public bool IsFecondationReady { get; private set; }
        public ulong Id { get; private set; }
        public ulong ExpirationDate { get; private set; }
        public uint Model { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string Owner { get; private set; } = string.Empty;
        public byte Level { get; private set; }
        public int ReproductionCount { get; private set; }
        public uint ReproductionCountMax { get; private set; }
        public ObjectEffectInteger[] Effects { get; private set; } = Array.Empty<ObjectEffectInteger>();
        public uint[] Capacities { get; private set; } = Array.Empty<uint>();

        public new void Serialize(DofusBinaryWriter writer)
        {
            throw new NotImplementedException();
        }

        public new void Deserialize(DofusBinaryReader reader)
        {
            base.Deserialize(reader);

            byte flags = reader.ReadByte();
            Sex = (flags & 0x01) != 0;
            IsRideable = (flags & 0x02) != 0;
            IsFeconded = (flags & 0x04) != 0;
            IsFecondationReady = (flags & 0x08) != 0;

            Id = (ulong)reader.Read7BitEncodedInt64();
            ExpirationDate = (ulong)reader.Read7BitEncodedInt64();
            Model = (uint)reader.Read7BitEncodedInt();
            Name = reader.ReadString();
            Owner = reader.ReadString();
            Level = reader.ReadByte();
            ReproductionCount = reader.Read7BitEncodedInt();
            ReproductionCountMax = (uint)reader.Read7BitEncodedInt();
            Effects = reader.ReadObjectCollection<ObjectEffectInteger>();
            Capacities = reader.ReadCollection(r => (uint)r.Read7BitEncodedInt());
        }
    }
}
