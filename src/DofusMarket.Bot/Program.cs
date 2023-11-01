using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Drawing;
using DofusMarket.Bot;
using DofusMarket.Bot.DataReader;
using DofusMarket.Bot.Input;
using DofusMarket.Bot.Internationalization;
using DofusMarket.Bot.Logging;
using DofusMarket.Bot.Sniffer;
using DofusMarket.Bot.Sniffer.Messages;
using Microsoft.Extensions.Logging;
using mtanksl.ActionMessageFormat;

var loggerFactory = LoggerFactory.Create(builder =>
{
    builder
        .AddSimpleConsole(options => options.SingleLine = true)
        .SetMinimumLevel(LogLevel.Debug);
});
LoggerProvider.Initialize(loggerFactory);

while (true)
{
    try
    {
        var allItemPrices = await CollectAllServerItemPricesAsync();
        KillDofus();

        string dofusPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Ankama", "Dofus");
        var dofusData = DofusData.New(dofusPath, new[] { "Servers", "Items", "ItemTypes" });

        var itemPriceMeasurements = ConvertItemPricesToMeasurements(allItemPrices, dofusData);
        DofusMarketMetrics.UpdateItemPriceMeasurements(itemPriceMeasurements);

        var itemAveragePrices = ReadServersAverageItemPrices();
        var itemAveragePriceMeasurements =
            ConvertServersItemPricesToMeasurements(itemAveragePrices, dofusData).ToArray();
        DofusMarketMetrics.UpdateAverageItemPriceMeasurements(itemAveragePriceMeasurements);
    }
    catch (Exception e)
    {
        LoggerProvider.CreateLogger<Program>().LogError(e, "An error occured while collecting prices");
    }

    await Task.Delay(TimeSpan.FromHours(10));
}

async Task<List<ItemPrice>> CollectAllServerItemPricesAsync()
{
    List<ItemPrice> itemPrices = new();
    RunDofus(
        Environment.GetEnvironmentVariable("ANKAMA_LOGIN")!,
        Environment.GetEnvironmentVariable("ANKAMA_PASSWORD")!);

    string networkDeviceId = Environment.GetEnvironmentVariable("NETWORK_DEVICE_ID")!;
    using DofusSniffer sniffer = new DofusSniffer(networkDeviceId).Start();
    NetworkMessageReader messageReader = new(sniffer);

    // Dofus
    var dofusWindow = Window.WaitForWindow("Dofus 2", TimeSpan.FromSeconds(10));
    dofusWindow.MoveWindow(new Rectangle(0, 0, 1920, 1080));

    // Dofus Server Selection
    var serverList = await messageReader.WaitForMessageAsync<ServerListMessage>();
    var servers = serverList.GameServerInformation
        .Where(s => s.Type != 2 && s.CharactersCount > 0) // idk what type 2 is but they are not displayed
        .OrderBy(s => s.Type)
        .ToArray();
    for (int i = 0; i < servers.Length; i += 1)
    {
        await Task.Delay(50);
        dofusWindow.MouseClick(new Point(739, 178)); // Order servers by name
        await Task.Delay(50);
        dofusWindow.MouseScroll(new Point(920, 260), 3); // Scroll up
        dofusWindow.MouseClick(new Point(920, 260)); // First server
        Keyboard.Send($"{{DOWN {i}}}");
        if (servers[i].Type >= 4) // Skip epic banner
        {
            Keyboard.Send("{DOWN}");
        }
        if (servers[i].Type >= 5) // Skip temporis banner
        {
            Keyboard.Send("{DOWN}");
        }
        Keyboard.Send("{ENTER}");

        // Dofus Character Selection
        var selectedServer = await messageReader.WaitForMessageAsync<SelectedServerDataMessage>();
        await messageReader.WaitForMessageAsync<CharactersListMessage>();
        await Task.Delay(50);
        dofusWindow.MouseClick(new Point(1256, 809)); // Play

        var mapInfo = await messageReader.WaitForMessageAsync<MapComplementaryInformationsDataMessage>();
        await Task.Delay(1000);

        var serverItemPrices = await CollectAllItemPricesFromCurrentMapAuctionHouseAsync(dofusWindow,
            messageReader, selectedServer.ServerId, mapInfo.MapId);
        itemPrices.AddRange(serverItemPrices);

        Keyboard.Send("{ESC}");
        await Task.Delay(200);
        dofusWindow.MouseClick(new Point(1006, 481)); // Change server
        await Task.Delay(200);
        dofusWindow.MouseClick(new Point(875, 551)); // Confirm

        await messageReader.WaitForMessageAsync<ServerListMessage>();
    }

    return itemPrices;
}

void RunDofus(string ankamaLogin, string ankamaPassword)
{
    var logger = LoggerProvider.CreateLogger<Program>();

    KillDofus();

    using var ankamaLauncherProcess = RunAnkamaLauncher();
    // It seems like launcher opens a window and quickly replace it so wait a little before getting the window handle.
    Thread.Sleep(2000);
    var ankamaLauncherWindow = Window.WaitForWindow("Ankama Launcher", TimeSpan.FromSeconds(5));

    ankamaLauncherWindow.Show();
    ankamaLauncherWindow.MoveWindow(new Rectangle(0, 0, 1920, 1080));
    ankamaLauncherWindow.Focus();
    Thread.Sleep(3000); // Wait for loading to end.

    // Session expired
    if (ankamaLauncherWindow.GetPixel(new Point(1133, 599)) == ColorTranslator.FromHtml("#335F69"))
    {
        logger.LogInformation("Session expired");

        ankamaLauncherWindow.MouseClick(new Point(1120, 670));
        Thread.Sleep(500);
        ankamaLauncherWindow.MouseClick(new Point(1840, 42));
        Thread.Sleep(500);
        ankamaLauncherWindow.MouseClick(new Point(1678, 546));
        Thread.Sleep(1000);
    }

    // Log out
    if (ankamaLauncherWindow.GetPixel(new Point(1804, 42)) == ColorTranslator.FromHtml("#00BB4B"))
    {
        logger.LogInformation("Logging out");

        ankamaLauncherWindow.MouseClick(new Point(1804, 42));
        Thread.Sleep(500);
        ankamaLauncherWindow.MouseClick(new Point(1689, 589));
        Thread.Sleep(2000);
    }

    logger.LogInformation("Logging in");
    ankamaLauncherWindow.MouseClick(new Point(245, 570)); // Ankama Login
    Thread.Sleep(1000);

    // Ankama Launcher Login
    ankamaLauncherWindow.MouseClick(new Point(86, 448), 3); // Ankama login input
    Keyboard.SendText(ankamaLogin);
    Keyboard.Send("{TAB}");
    Keyboard.SendText(ankamaPassword);
    Keyboard.Send("{TAB 3}{SPACE}");
    Thread.Sleep(2000);

    // Ankama Launcher 2FA
    if (ankamaLauncherWindow.GetPixel(new Point(244, 492)) == ColorTranslator.FromHtml("#001519"))
    {
        throw new Exception("2FA expected");
    }

    // Ankama Launcher Dofus
    ankamaLauncherWindow.WaitForPixel(new Point(386, 424), ColorTranslator.FromHtml("#FFFFFF"), TimeSpan.FromMinutes(5)); // Play button
    ankamaLauncherWindow.MouseClick(new Point(386, 424)); // Play
}

async Task<List<ItemPrice>> CollectAllItemPricesFromCurrentMapAuctionHouseAsync(Window dofusWindow,
    NetworkMessageReader messageReader, uint serverId, long mapId)
{
    var sw = Stopwatch.StartNew();

    var auctionHousePos = mapId switch
    {
        73400322 => new Point(1355, 636), // Koalak
        153879299 => new Point(1100, 360), // Incarnam
        _ => throw new Exception($"Unexpected map id {mapId} on server {serverId}"),
    };

    dofusWindow.MouseClick(auctionHousePos); // Auction house
    var buyerDescriptor = (await messageReader.WaitForMessageAsync<ExchangeStartedBidBuyerMessage>()).BuyerDescriptor;

    List<ItemPrice> itemPrices = new();

    await ClickAllItemFromScrollableListAsync(
        dofusWindow,
        itemCount: 59, // Different from BuyerDescriptor.Types.Length for some reason.
        itemVisible: 16,
        itemPerScroll: 3,
        itemLineHeightPx: 32,
        firstItemPosition: new Point(350, 355),
        itemFunc: async () =>
        {
            var exchangeTypes = await messageReader.WaitForMessageAsync<ExchangeTypesExchangerDescriptionForUserMessage>();

            await ClickAllItemFromScrollableListAsync(
                dofusWindow,
                itemCount: exchangeTypes.TypeDescription.Length,
                itemVisible: 14,
                itemPerScroll: 3,
                itemLineHeightPx: 48,
                firstItemPosition: new Point(660, 210),
                itemFunc: async () =>
                {
                    var exchangeTypesItems = await messageReader.WaitForMessageAsync<ExchangeTypesItemsExchangerDescriptionForUserMessage>();
                    for (int i = 0; i < buyerDescriptor.Quantities.Length; i += 1)
                    {
                        long price = (long)exchangeTypesItems.ItemTypeDescriptions
                            .Select(o => (long)o.Prices[i])
                            .Where(p => p != 0)
                            .DefaultIfEmpty()
                            .Average();
                        if (price == 0) // 0 means that the item is not available for this set size.
                        {
                            continue;
                        }

                        itemPrices.Add(new ItemPrice((int)serverId, exchangeTypesItems.ObjectGid,
                            (int)exchangeTypesItems.ObjectType, (int)buyerDescriptor.Quantities[i], price));
                    }
                });
        });

    Keyboard.Send("{ESC}"); // Close auction house

    LoggerProvider.CreateLogger<Program>()
        .LogInformation("Collected {0} item prices from server {1} in {2} minutes",
            itemPrices.Count, serverId, (int)sw.Elapsed.TotalMinutes);

    return itemPrices;
}

async Task ClickAllItemFromScrollableListAsync(Window dofusWindow, int itemCount, int itemVisible, int itemPerScroll,
    int itemLineHeightPx, Point firstItemPosition, Func<Task> itemFunc)
{
    Point currentItemPos = firstItemPosition;
    for (int itemTypeIdx = 0; itemTypeIdx < itemCount; itemTypeIdx += itemPerScroll)
    {
        int itemRemaining = itemCount - itemTypeIdx;
        int itemsToScan;
        if (itemTypeIdx == 0) // First iteration.
        {
            itemsToScan = Math.Min(itemVisible, itemRemaining);
        }
        else if (itemRemaining > itemPerScroll)
        {
            itemsToScan = itemPerScroll;
        }
        else // Last iteration.
        {
            itemsToScan = itemRemaining;
            currentItemPos.Y += (itemPerScroll - itemsToScan) * itemLineHeightPx;
        }

        for (int j = 0; j < itemsToScan; j += 1)
        {
            // Select item type.
            dofusWindow.MouseClick(currentItemPos);
            await Task.Delay(500);

            await itemFunc();

            // Unselect item type.
            dofusWindow.MouseClick(currentItemPos);
            await Task.Delay(500);

            currentItemPos.Y += itemLineHeightPx;
        }

        currentItemPos.Y -= itemPerScroll * itemLineHeightPx;
        if (itemTypeIdx == 0)
        {
            itemTypeIdx += itemVisible - itemPerScroll;
        }

        dofusWindow.MouseScroll(currentItemPos, -1);
    }
}

void KillDofus()
{
    foreach (var dofusProcess in Process.GetProcessesByName("dofus"))
    {
        dofusProcess.Kill();
        dofusProcess.WaitForExit(TimeSpan.FromSeconds(10));
    }
}

Process RunAnkamaLauncher()
{
    string ankamaLauncherExePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
        "Ankama", "Ankama Launcher", "Ankama Launcher.exe");
    return Process.Start(new ProcessStartInfo(ankamaLauncherExePath)
    {
        RedirectStandardOutput = false,
        RedirectStandardError = false,
    })!;
}

Measurement<long>[] ConvertItemPricesToMeasurements(List<ItemPrice> itemPrices, DofusData data)
{
    var serversData = data.GetDataForType("Servers");
    var itemsData = data.GetDataForType("Items");
    var itemTypesData = data.GetDataForType("ItemTypes");

    var measurements = new Measurement<long>[itemPrices.Count];
    for (int i = 0; i < itemPrices.Count; i += 1)
    {
        var itemPrice = itemPrices[i];
        var serverTextId = (int)serversData[itemPrice.ServerId]["nameId"]!;
        var itemTextId = (int)itemsData[itemPrice.ObjectId]["nameId"]!;
        var itemTypeTextId = (int)itemTypesData[itemPrice.ObjectTypeId]["nameId"]!;

        measurements[i] = new Measurement<long>(itemPrice.Price, new KeyValuePair<string, object?>[]
        {
            new("server.id", itemPrice.ServerId),
            new("server.name_fr", data.GetText(serverTextId, DofusLanguages.French)),
            new("server.name_canonical_fr", data.GetUndiacriticText(serverTextId, DofusLanguages.French)),
            new("item.id", itemPrice.ObjectId),
            new("item.name_fr", data.GetText(itemTextId, DofusLanguages.French)),
            new("item.name_canonical_fr", data.GetUndiacriticText(itemTextId, DofusLanguages.French)),
            new("item.level", (int)itemsData[itemPrice.ObjectId]["level"]!),
            new("item_type.id", itemPrice.ObjectTypeId),
            new("item_type.name_fr", data.GetText(itemTypeTextId, DofusLanguages.French)),
            new("item_type.name_canonical_fr", data.GetUndiacriticText(itemTypeTextId, DofusLanguages.French)),
            new("quantity", itemPrice.Quantity),
        });
    }

    return measurements;
}

Dictionary<string, Dictionary<int, long>> ReadServersAverageItemPrices()
{
    Dictionary<string, Dictionary<int, long>> serversPrices = new();

    var logger = LoggerProvider.CreateLogger<Program>();

    string itemAveragePricesPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "Dofus", "itemAveragePrices.dat");
    var reader = new AmfReader(File.ReadAllBytes(itemAveragePricesPath));
    var root = (Amf3Object)reader.ReadAmf3();
    foreach (KeyValuePair<string, object> server in root.DynamicMembersAndValues)
    {
        string serverName = server.Key;
        var serverData = ((Amf3Object)server.Value).DynamicMembersAndValues;
        var lastUpdate = (DateTime)serverData["lastUpdate"];
        var itemPrices = (Dictionary<object, object>)serverData["items"];

        if (DateTime.Now - lastUpdate > TimeSpan.FromDays(1))
        {
            logger.LogWarning($"Ignoring server '{serverName}' with a too old last update '{lastUpdate}'");
            continue;
        }

        serversPrices[serverName] = new Dictionary<int, long>();
        foreach (var itemPrice in itemPrices)
        {
            int itemId = int.Parse((string)itemPrice.Key);
            long price = itemPrice.Value switch
            {
                int p => p,
                double p => (long)p,
                _ => throw new Exception($"Unexpected type for price: {itemPrice.Value.GetType()}")
            };
            serversPrices[serverName][itemId] = price;
        }

        logger.LogInformation($"server: '{serverName}', last update: {lastUpdate}, item prices: {itemPrices.Count}");
    }

    return serversPrices;
}

IEnumerable<Measurement<long>> ConvertServersItemPricesToMeasurements(
    Dictionary<string, Dictionary<int, long>> serversPrices,
    DofusData data)
{
    var serversData = data.GetDataForType("Servers");
    var itemsData = data.GetDataForType("Items");
    var itemTypesData = data.GetDataForType("ItemTypes");

    foreach (var serverPrices in serversPrices)
    {
        if (!TryGetServerIdFromName(serverPrices.Key, data, out int serverId))
        {
            continue;
        }

        var serverData = serversData[serverId];
        foreach (var itemPrice in serverPrices.Value)
        {
            int serverNameId = (int)serverData["nameId"]!;
            string serverNameFr = data.GetText(serverNameId, DofusLanguages.French);
            string serverNameCanonicalFr = data.GetUndiacriticText(serverNameId, DofusLanguages.French);

            int itemId = itemPrice.Key;
            int itemNameId = (int)itemsData[itemId]["nameId"]!;
            string itemNameFr = data.GetText(itemNameId, DofusLanguages.French);
            string itemNameCanonicalFr = data.GetUndiacriticText(itemNameId, DofusLanguages.French);

            int itemLevel = (int)itemsData[itemId]["level"]!;

            int itemTypeId = (int)itemsData[itemId]["typeId"]!;
            int itemTypeNameId = (int)itemTypesData[itemTypeId]["nameId"]!;
            string itemTypeNameFr = data.GetText(itemTypeNameId, DofusLanguages.French);
            string itemTypeNameCanonicalFr = data.GetUndiacriticText(itemTypeNameId, DofusLanguages.French);

            yield return new Measurement<long>(itemPrice.Value, new KeyValuePair<string, object?>[]
            {
                new("server.id", serverId),
                new("server.name_fr", serverNameFr),
                new("server.name_canonical_fr", serverNameCanonicalFr),
                new("item.id", itemId),
                new("item.name_fr", itemNameFr),
                new("item.name_canonical_fr", itemNameCanonicalFr),
                new("item.level", itemLevel),
                new("item_type.id", itemTypeId),
                new("item_type.name_fr", itemTypeNameFr),
                new("item_type.name_canonical_fr", itemTypeNameCanonicalFr),
            });
        }
    }
}

bool TryGetServerIdFromName(
    string name,
    DofusData data,
    out int serverId)
{
    var serversData = data.GetDataForType("Servers");

    foreach (int textId in data.GetTextIds(name))
    {
        foreach (var serverData in serversData)
        {
            if (serverData.Value["nameId"] is int nameId && nameId == textId)
            {
                serverId = serverData.Key;
                return true;
            }
        }
    }

    serverId = default;
    return false;
}

record ItemPrice(int ServerId, int ObjectId, int ObjectTypeId, int Quantity, long Price);