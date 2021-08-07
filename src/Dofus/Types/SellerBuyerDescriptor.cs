using System;
using Dofus.Messages;
using Dofus.Serialization;

namespace Dofus.Types
{
    public class SellerBuyerDescriptor : INetworkMessage
    {
        internal static int MessageId => 5463;

        public uint[] Quantities { get; private set; } = Array.Empty<uint>();
        public uint[] Types { get; private set; } = Array.Empty<uint>();
        public float TaxPercentage { get; private set; }
        public float TaxModificationPercentage { get; private set; }
        public byte MaxItemLevel { get; private set; }
        public uint MaxItemPerAccount { get; private set; }
        public int NpcContextualId { get; private set; }
        public uint UnsoldDelay { get; private set; }

        public void Serialize(DofusBinaryWriter writer)
        {
            throw new NotImplementedException();
        }

        public void Deserialize(DofusBinaryReader reader)
        {
            Quantities = reader.ReadCollection(r => (uint)r.Read7BitEncodedInt());
            Types = reader.ReadCollection(r => (uint)r.Read7BitEncodedInt());
            TaxPercentage = reader.ReadSingle();
            TaxModificationPercentage = reader.ReadSingle();
            MaxItemLevel = reader.ReadByte();
            MaxItemPerAccount = (uint)reader.Read7BitEncodedInt();
            NpcContextualId = reader.ReadInt32();
            UnsoldDelay = (uint)reader.Read7BitEncodedInt();
        }
    }
}
