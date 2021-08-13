using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dofus;
using Dofus.Messages;
using DofusMarket.Bot.Models;
using DofusMarket.Bot.Services;
using Microsoft.Extensions.Logging;

namespace DofusMarket.Bot.Frames
{
    internal class ItemPricesCollectorFrame : Frame
    {
        private static readonly (long mapId, uint npcId)[] AuctionHouses =
        {
            (191104004, 515220), // Astrub.
            (146741, 515264), // Bonta.
        };
        private static readonly int[] StackSizes = { 1, 10, 100 };

        private readonly int _serverId;
        private readonly DofusMetrics _metrics;

        public ItemPricesCollectorFrame(int serverId, DofusMetrics metrics)
        {
            _serverId = serverId;
            _metrics = metrics;
        }

        public override async Task ProcessAsync(CancellationToken cancellationToken)
        {
            var currentMap = await ReceiveMessageAsync<CurrentMapMessage>();
            var auctionHouse = AuctionHouses.FirstOrDefault(h => h.mapId == currentMap.MapId);
            if (auctionHouse == default)
            {
                Logger.LogError("Character is not at a known auction house");
                return;
            }

            await SendMessageAsync(new MapInformationsRequestMessage { MapId = currentMap.MapId });

            var mapData = await ReceiveMessageAsync<MapComplementaryInformationsDataMessage>();
            uint skillInstanceUid = mapData.InteractiveElements.First(e => e.ElementId == auctionHouse.npcId).EnabledSkills[0].SkillInstanceUid;
            await SendMessageAsync(new InteractiveUseRequestMessage
            {
                ElemId = auctionHouse.npcId,
                SkillInstanceUid = skillInstanceUid,
            });

            switch (await ReceiveAnyMessageAsync(typeof(InteractiveUseErrorMessage),
                typeof(InteractiveUsedMessage)))
            {
                case InteractiveUseErrorMessage:
                    Logger.LogError("Interactive use error");
                    return;
            }

            var exchange = await ReceiveMessageAsync<ExchangeStartedBidBuyerMessage>();
            await ReceiveMessageAsync<BasicNoOperationMessage>();
            foreach (uint itemTypeId in exchange.BuyerDescriptor.Types)
            {
                await SendMessageAsync(new ExchangeBidHouseTypeMessage
                {
                    Type = itemTypeId,
                    Follow = true,
                });

                var exchangeType = await ReceiveMessageAsync<ExchangeTypesExchangerDescriptionForUserMessage>();
                await ReceiveMessageAsync<BasicNoOperationMessage>();
                foreach (uint itemId in exchangeType.TypeDescription)
                {
                    await SendMessageAsync(new ExchangeBidHouseSearchMessage
                    {
                        GenId = (short)itemId,
                        Follow = true,
                    });

                    switch (await ReceiveAnyMessageAsync(typeof(ExchangeTypesItemsExchangerDescriptionForUserMessage),
                        typeof(BasicNoOperationMessage)))
                    {
                        case ExchangeTypesItemsExchangerDescriptionForUserMessage item:
                            for (int i = 0; i < StackSizes.Length; i += 1)
                            {
                                int price = (int)item.ItemTypeDescriptions
                                    .Select(o => (int)o.Prices[i])
                                    .Where(p => p != 0)
                                    .DefaultIfEmpty()
                                    .Average();
                                if (price == 0) // 0 means that the item is not available for this set size.
                                {
                                    continue;
                                }

                                _metrics.WriteItemPrice(new ItemPrice(_serverId, (int)itemId, (int)itemTypeId,
                                    StackSizes[i], price));
                            }
                            await ReceiveMessageAsync<BasicNoOperationMessage>();
                            break;

                        case BasicNoOperationMessage:
                            Logger.LogInformation("Item {0} not found?", itemId);
                            break;
                    }

                    // There is a maximum of items we can follow, so make sure to unfollow them.
                    await SendMessageAsync(new ExchangeBidHouseSearchMessage
                    {
                        GenId = (short)itemId,
                        Follow = false,
                    });
                    await ReceiveMessageAsync<BasicNoOperationMessage>();

                    await Task.Delay(TimeSpan.FromMilliseconds(500), cancellationToken);
                }
            }

            await SendMessageAsync(new LeaveDialogRequestMessage());
        }
    }
}
