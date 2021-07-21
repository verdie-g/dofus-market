using Dofus.Serialization;

namespace Dofus.Messages
{
    public class ExchangeBidHouseSearchMessage : INetworkMessage
    {
        internal static int MessageId => 4493;

        public short GenId { get; init; }
        public bool Follow { get; init; }

        public void Serialize(DofusBinaryWriter writer)
        {
            writer.Write7BitEncodedInt(GenId);
            writer.Write(Follow);
        }

        public void Deserialize(DofusBinaryReader reader)
        {
            throw new System.NotImplementedException();
        }
    }
}
