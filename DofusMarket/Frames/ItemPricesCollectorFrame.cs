using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dofus;
using Dofus.Messages;
using Microsoft.Extensions.Logging;

namespace DofusMarket.Frames
{
    public class ItemPricesCollectorFrame : Frame
    {
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

            List<ExchangeTypesItemsExchangerDescriptionForUserMessage> items = new();

            var exchange = await ReceiveMessageAsync<ExchangeStartedBidBuyerMessage>();
            await ReceiveMessageAsync<BasicNoOperationMessage>();
            foreach (uint itemCategoryId in exchange.BuyerDescriptor.Types)
            {
                await SendMessageAsync(new ExchangeBidHouseTypeMessage
                {
                    Type = itemCategoryId,
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
                            items.Add(item);
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
        }
    }
}
