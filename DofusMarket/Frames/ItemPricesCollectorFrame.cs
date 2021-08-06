using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dofus;
using Dofus.Messages;
using DofusMarket.Services;
using Microsoft.Extensions.Logging;

namespace DofusMarket.Frames
{
    internal class ItemPricesCollectorFrame : Frame
    {
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
            await SendMessageAsync(new MapInformationsRequestMessage { MapId = currentMap.MapId });

            var mapData = await ReceiveMessageAsync<MapComplementaryInformationsDataMessage>();
            uint elementId = 515264;
            uint skillInstanceUid = mapData.InteractiveElements.First(e => e.ElementId == elementId).EnabledSkills[0].SkillInstanceUid;
            await SendMessageAsync(new InteractiveUseRequestMessage
            {
                ElemId = elementId,
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
                                    .Where(o => o.Prices[i] != 0)
                                    .Average(o => (int)o.Prices[i]);
                                if (price == 0) // 0 means that the item is not available for this set size.
                                {
                                    continue;
                                }

                                int stackSize = StackSizes[i];
                                _metrics.WriteItemPrice(_serverId, (int)itemId, (int)itemTypeId, stackSize, price);
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
