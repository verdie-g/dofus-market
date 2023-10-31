using DofusMarket.Bot.Serialization;
using DofusMarket.Bot.Sniffer.Types;

namespace DofusMarket.Bot.Sniffer.Messages;

internal record ExchangeStartedBidBuyerMessage : INetworkMessage
{
    internal static ushort MessageId => 69;

    public SellerBuyerDescriptor BuyerDescriptor { get; private set; } = default!;

    public void Deserialize(DofusBinaryReader reader)
    {
        BuyerDescriptor = reader.ReadObject<SellerBuyerDescriptor>();
    }
}