using System;
using Dofus.Serialization;
using Dofus.Types;

namespace Dofus.Messages
{
    public class ExchangeTypesItemsExchangerDescriptionForUserMessage : INetworkMessage
    {
        internal static ushort MessageId => 6000;

        public uint ObjectType { get; private set; }
        public BidExchangerObjectInfo[] ItemTypeDescriptions { get; private set; } = Array.Empty<BidExchangerObjectInfo>();

        public void Serialize(DofusBinaryWriter writer)
        {
            throw new NotImplementedException();
        }

        public void Deserialize(DofusBinaryReader reader)
        {
            ObjectType = reader.ReadUInt32();
            ItemTypeDescriptions = reader.ReadObjectCollection<BidExchangerObjectInfo>();
        }
    }
}
