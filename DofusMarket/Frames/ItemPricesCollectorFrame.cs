using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dofus;
using Dofus.Internationalization;
using Dofus.Messages;
using DofusMarket.Services;
using Microsoft.Extensions.Logging;

namespace DofusMarket.Frames
{
    internal class ItemPricesCollectorFrame : Frame
    {
        private static readonly int[] StackSizes = { 1, 10, 100 };

        private readonly string _serverName;
        private readonly DofusMetrics _metrics;
        private readonly DofusData _dofusData;
        private readonly DofusTexts _dofusTexts;

        public ItemPricesCollectorFrame(int serverId, DofusMetrics metrics, DofusData dofusData, DofusTexts dofusTexts)
        {
            _metrics = metrics;
            _dofusData = dofusData;
            _dofusTexts = dofusTexts;
            _serverName = GetServerName(serverId);
        }

        public override async Task ProcessAsync(CancellationToken cancellationToken)
        {
            var currentMap = await ReceiveMessageAsync<CurrentMapMessage>();
            await SendMessageAsync(new MapInformationsRequestMessage { MapId = currentMap.MapId });

            var mapData = await ReceiveMessageAsync<MapComplementaryInformationsDataMessage>();
            uint elementId = 515220;
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
                            string itemName = GetItemName((int)itemId);
                            string itemTypeName = GetItemTypeName((int)itemTypeId);
                            for (int i = 0; i < item.ItemTypeDescriptions[0].Prices.Length; i += 1)
                            {
                                int price = (int)item.ItemTypeDescriptions[0].Prices[i];
                                if (price == 0) // 0 means that the item is not available for this set size.
                                {
                                    continue;
                                }

                                int stackSize = StackSizes[i];
                                _metrics.EmitItemPrice(_serverName, itemName, itemTypeName, stackSize, price);
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

        private string GetItemName(int id)
        {
            var item = _dofusData.GetData(id, "Items");
            return _dofusTexts.GetText((int)item["nameId"]!, DofusLanguages.French);
        }

        private string GetItemTypeName(int id)
        {
            var itemType = _dofusData.GetData(id, "ItemTypes");
            return _dofusTexts.GetText((int)itemType["nameId"]!, DofusLanguages.French);
        }

        private string GetServerName(int id)
        {
            var server = _dofusData.GetData(id, "Servers");
            return _dofusTexts.GetText((int)server["nameId"]!, DofusLanguages.French);
        }
    }
}
