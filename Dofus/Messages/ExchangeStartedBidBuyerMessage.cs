using Dofus.Serialization;
using Dofus.Types;

namespace Dofus.Messages
{
    public class ExchangeStartedBidBuyerMessage : INetworkMessage
    {
        internal static int MessageId => 4958;

        public SellerBuyerDescriptor BuyerDescriptor { get; private set; } = new();

        public void Serialize(DofusBinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public void Deserialize(DofusBinaryReader reader)
        {
            BuyerDescriptor = reader.ReadObject<SellerBuyerDescriptor>();
        }
    }
}
