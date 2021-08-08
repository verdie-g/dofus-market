using Dofus.Serialization;

namespace Dofus.Messages
{
    public class ExchangeBidHouseGenericItemAddedMessage : INetworkMessage
    {
        internal static int MessageId => 5133;

        public ushort ObjGenericId { get; private set; }

        public void Serialize(DofusBinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public void Deserialize(DofusBinaryReader reader)
        {
            ObjGenericId = (ushort)reader.Read7BitEncodedInt();
        }
    }
}
