using System;
using Dofus.Serialization;

namespace Dofus.Messages
{
    public class ExchangeTypesExchangerDescriptionForUserMessage : INetworkMessage
    {
        internal static int MessageId => 1423;

        public uint ObjectType { get; private set; }
        public uint[] TypeDescription { get; private set; } = Array.Empty<uint>();

        public void Serialize(DofusBinaryWriter writer)
        {
            throw new NotImplementedException();
        }

        public void Deserialize(DofusBinaryReader reader)
        {
            ObjectType = reader.ReadUInt32();
            TypeDescription = reader.ReadCollection(r => (uint)r.Read7BitEncodedInt());
        }
    }
}
