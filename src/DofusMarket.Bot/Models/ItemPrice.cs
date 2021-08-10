namespace DofusMarket.Bot.Models
{
    public record ItemPrice(int ServerId, int ItemId, int ItemTypeId, int StackSize, int Price);
}
