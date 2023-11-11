using System.Diagnostics;
using System.Drawing;
using DofusMarket.Bot;
using DofusMarket.Bot.DataReader;
using DofusMarket.Bot.Input;
using DofusMarket.Bot.Logging;
using DofusMarket.Bot.Sniffer;
using DofusMarket.Bot.Sniffer.Messages;
using DofusMarket.Bot.Sniffer.Types;
using Microsoft.Extensions.Logging;
using mtanksl.ActionMessageFormat;
using Serilog;
using Serilog.Sinks.Grafana.Loki;
using ILogger = Microsoft.Extensions.Logging.ILogger;

InitializeLogging();

var l = LoggerProvider.CreateLogger<Program>();
while (true)
{
    TimeSpan delay;
    try
    {
        KillAll("dofus");

        string dofusPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Ankama", "Dofus");
        var dofusData = DofusData.New(dofusPath, new[] { "Servers", "Items", "ItemTypes" });
        DofusMarketMetrics metrics = new(dofusData);

        metrics.WriteItemAveragePrices(ReadServersAverageItemPrices(dofusData));

        var sw = Stopwatch.StartNew();
        await CollectAllServerItemPricesAsync(metrics);
        l.LogInformation("Collected item prices from all servers {1} minutes",
            (int)sw.Elapsed.TotalMinutes);

        metrics.WriteItemAveragePrices(ReadServersAverageItemPrices(dofusData));

        delay = TimeSpan.FromHours(6);
    }
    catch (Exception e)
    {
        l.LogError(e, "An error occured while collecting prices");
        delay = TimeSpan.FromHours(3);
    }

    l.LogInformation($"Waiting {delay} for next run");
    await Task.Delay(delay);
}

async Task CollectAllServerItemPricesAsync(DofusMarketMetrics metrics)
{
    var logger = LoggerProvider.CreateLogger<Program>();

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
    List<GameServerInformation?> servers = serverList.GameServerInformation
        .Where(s => s.Type != 2 && s.CharactersCount > 0) // idk what type 2 is but they are not displayed
        .OrderBy(s => s.Type)
        .ToList()!;
    byte lastServerType = byte.MaxValue;
    for (int i = 0; i < servers.Count; i += 1)
    {
        var serverType = servers[i]!.Type;
        if (serverType != lastServerType)
        {
            servers.Insert(i, null); // Insert null to represent the server type banner.
            i += 1;
        }

        lastServerType = serverType;
    }
    await Task.Delay(250);

    await ScrollListAsync(
        dofusWindow,
        itemCount: servers.Count,
        itemVisible: 12,
        itemPerScroll: 3,
        itemLineHeightPx: 45,
        firstItemPosition: new Point(900, 220),
        itemFunc: async (serverIdx, serverPosition) =>
        {
            if (servers[serverIdx] == null)
            {
                return;
            }

            using var serverLoggingScope = logger.BeginScope(new KeyValuePair<string, object>[]
            {
                new("server.id", servers[serverIdx]!.Id),
            });

            await Task.Delay(50);
            dofusWindow.MouseClick(new Point(739, 178), debugName: "Order servers by name");
            await Task.Delay(50);
            dofusWindow.MouseClick(serverPosition, 2);

            // Dofus Character Selection
            var selectedServer = await messageReader.WaitForMessageAsync<SelectedServerDataMessage>();
            await messageReader.WaitForMessageAsync<CharactersListMessage>();
            await Task.Delay(500);
            dofusWindow.MouseClick(new Point(1256, 809), debugName: "Play");

            var mapInfo = await messageReader.WaitForMessageAsync<MapComplementaryInformationsDataMessage>();
            await Task.Delay(1500);

            await CollectAllItemPricesFromCurrentMapAuctionHouseAsync(dofusWindow,
                messageReader, selectedServer.ServerId, mapInfo.MapId, metrics);

            dofusWindow.MouseClick(new Point(1882, 16), debugName: "Exit");
            await Task.Delay(200);
            dofusWindow.MouseClick(new Point(1006, 481), debugName: "Change server");
            await Task.Delay(200);
            dofusWindow.MouseClick(new Point(875, 551), debugName: "Confirm");

            await messageReader.WaitForMessageAsync<ServerListMessage>();
            await Task.Delay(250);
        });

    KillAll("dofus");
}

void RunDofus(string ankamaLogin, string ankamaPassword)
{
    var logger = LoggerProvider.CreateLogger<Program>();

    KillAll("dofus");

    using var ankamaLauncherProcess = RunAnkamaLauncher();
    // It seems like launcher opens a window and quickly replace it so wait a little before getting the window handle.
    Thread.Sleep(2000);
    var ankamaLauncherWindow = Window.WaitForWindow("Ankama Launcher", TimeSpan.FromSeconds(5));

    ankamaLauncherWindow.Focus();
    ankamaLauncherWindow.MoveWindow(new Rectangle(0, 0, 1920, 1080));

    Thread.Sleep(4000); // Wait for loading to end (TODO: find a better way than a sleep).

#if false
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
    ankamaLauncherWindow.Focus(); // Focus before sending text.
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

    // Session expired
    if (ankamaLauncherWindow.GetPixel(new Point(985, 505)) == ColorTranslator.FromHtml("#001519"))
    {
        logger.LogInformation("Session expired");

        ankamaLauncherWindow.MouseClick(new Point(985, 505));
        Keyboard.SendText(ankamaPassword);
        ankamaLauncherWindow.MouseClick(new Point(916, 598));
        Thread.Sleep(1000);
    }

    // Ankama Launcher Dofus
    ankamaLauncherWindow.WaitForPixel(new Point(386, 424), ColorTranslator.FromHtml("#FFFFFF"), TimeSpan.FromMinutes(5)); // Play button
#endif
    // If an ankama launcher update is required it will restart the launcher which will make the current run fail
    // but it's ok.
    ankamaLauncherWindow.MouseClick(new Point(962, 551), debugName: "Update");
    ankamaLauncherWindow.MouseClick(new Point(386, 424), debugName: "Play");
}

async Task CollectAllItemPricesFromCurrentMapAuctionHouseAsync(Window dofusWindow,
    NetworkMessageReader messageReader, uint serverId, long mapId, DofusMarketMetrics metrics)
{
    var logger = LoggerProvider.CreateLogger<Program>();
    var sw = Stopwatch.StartNew();

    var auctionHousePos = mapId switch
    {
        73400322 => new Point(1355, 636), // Koalak
        153879299 => new Point(1100, 360), // Incarnam
        90707714 => new Point(994, 272), // Sufokia
        _ => throw new Exception($"Unexpected map id {mapId} on server {serverId}"),
    };

    dofusWindow.MouseClick(auctionHousePos, debugName: "Open auction house");
    var buyerDescriptor = (await messageReader.WaitForMessageAsync<ExchangeStartedBidBuyerMessage>()).BuyerDescriptor;
    await Task.Delay(TimeSpan.FromMilliseconds(1500));

    await ScrollListAsync(
        dofusWindow,
        itemCount: 59, // Different from BuyerDescriptor.Types.Length for some reason.
        itemVisible: 16,
        itemPerScroll: 3,
        itemLineHeightPx: 32,
        firstItemPosition: new Point(380, 355),
        itemFunc: async (_, itemTypePosition) =>
        {
            dofusWindow.MouseClick(itemTypePosition, debugName: "Select item type");
            var exchangeTypes = await messageReader.WaitForMessageAsync<ExchangeTypesExchangerDescriptionForUserMessage>();
            await Task.Delay(TimeSpan.FromMilliseconds(1000));

            await ScrollListAsync(
                dofusWindow,
                itemCount: exchangeTypes.TypeDescription.Length,
                itemVisible: 14,
                itemPerScroll: 3,
                itemLineHeightPx: 48,
                firstItemPosition: new Point(660, 210),
                itemFunc: async (itemIndex, itemPosition) =>
                {
                    var exchangeTypesItems = await SelectItemAsync(dofusWindow, messageReader, logger, itemIndex, itemPosition);

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

                        metrics.WriteItemPrice(new ItemPrice((int)serverId, exchangeTypesItems.ObjectGid,
                            (int)buyerDescriptor.Quantities[i], price));
                    }

                    await UnselectItemAsync(dofusWindow, messageReader, logger, itemIndex, itemPosition);
                });

            dofusWindow.MouseClick(itemTypePosition, debugName: "Unselect item type");
            await Task.Delay(TimeSpan.FromMilliseconds(100));
        });

    dofusWindow.MouseClick(new Point(1203, 65), debugName: "Close auction house");

    logger.LogInformation("Collected item prices from server {0} in {1} minutes", serverId,
        (int)sw.Elapsed.TotalMinutes);
}

async Task<ExchangeTypesItemsExchangerDescriptionForUserMessage> SelectItemAsync(
    Window dofusWindow, NetworkMessageReader messageReader, ILogger logger, int itemIndex, Point itemPosition)
{
    dofusWindow.MouseClick(itemPosition, debugName: "Select item at index " + itemIndex);
    ExchangeBidHouseSearchMessage? search = null;
    try
    {
        search = await messageReader.WaitForMessageAsync<ExchangeBidHouseSearchMessage>(TimeSpan.FromSeconds(2));
    }
    catch (TaskCanceledException)
    {
    }

    if (search == null)
    {
        logger.LogWarning($"Selecting an item did not send a {nameof(ExchangeBidHouseSearchMessage)}"
                          + ". Retrying the select");
        dofusWindow.MouseClick(itemPosition, debugName: "Select again item at index " + itemIndex);
        search = await messageReader.WaitForMessageAsync<ExchangeBidHouseSearchMessage>();
    }

    var exchangeTypesItems = await messageReader.WaitForMessageAsync<ExchangeTypesItemsExchangerDescriptionForUserMessage>();
    if (exchangeTypesItems.ObjectGid != search.ObjectGid)
    {
        logger.LogWarning($"Received {nameof(ExchangeTypesItemsExchangerDescriptionForUserMessage)}"
                          + $" with item id {exchangeTypesItems.ObjectGid} but expected {search.ObjectGid}."
                          + $" Skipping this message hoping it was just a duplicate response from the last request");

        exchangeTypesItems = await messageReader.WaitForMessageAsync<ExchangeTypesItemsExchangerDescriptionForUserMessage>();
        if (exchangeTypesItems.ObjectGid != search.ObjectGid)
        {
            throw new Exception($"Received {nameof(ExchangeTypesItemsExchangerDescriptionForUserMessage)}"
                                + $" with item id {exchangeTypesItems.ObjectGid} but expected {search.ObjectGid}.");
        }
    }

    await Task.Delay(TimeSpan.FromMilliseconds(200));
    return exchangeTypesItems;
}

async Task UnselectItemAsync(Window dofusWindow, NetworkMessageReader messageReader, ILogger logger, int itemIndex,
    Point itemPosition)
{
    dofusWindow.MouseClick(itemPosition, debugName: "Unselect item at index " + itemIndex);
    ExchangeBidHouseSearchMessage? search = null;
    try
    {
        search = await messageReader.WaitForMessageAsync<ExchangeBidHouseSearchMessage>(TimeSpan.FromSeconds(2));
    }
    catch (TaskCanceledException)
    {
    }

    if (search == null)
    {
        logger.LogWarning($"Unselecting an item did not send a {nameof(ExchangeBidHouseSearchMessage)}"
                          + ". Retrying the unselect");
        dofusWindow.MouseClick(itemPosition, debugName: "Unselect again item at index " + itemIndex);
        search = await messageReader.WaitForMessageAsync<ExchangeBidHouseSearchMessage>();
    }

    // Unselecting is supposed to send a request if Follow=false but sometimes it's true and the item
    // doesn't collapse. It looks like sometimes the item just takes forever to expand.
    if (search.Follow)
    {
        logger.LogWarning(
            $"Sent {nameof(ExchangeBidHouseSearchMessage)} with {nameof(ExchangeBidHouseSearchMessage.Follow)}=true"
            + " when trying to unselect. Retrying the unselect");
        await Task.Delay(TimeSpan.FromMilliseconds(500));
        dofusWindow.MouseClick(itemPosition, debugName: "Unselect again item at index " + itemIndex);
        search = await messageReader.WaitForMessageAsync<ExchangeBidHouseSearchMessage>();
        if (search.Follow)
        {
            throw new Exception(
                $"Sent {nameof(ExchangeBidHouseSearchMessage)} with {nameof(ExchangeBidHouseSearchMessage.Follow)}=true"
                + " when trying to unselect");
        }
    }

    await messageReader.WaitForMessageAsync<ExchangeTypesItemsExchangerDescriptionForUserMessage>();
    await Task.Delay(TimeSpan.FromMilliseconds(400));
}

async Task ScrollListAsync(Window dofusWindow, int itemCount, int itemVisible, int itemPerScroll,
    int itemLineHeightPx, Point firstItemPosition, Func<int, Point, Task> itemFunc)
{
    Point currentItemPos = firstItemPosition;
    for (int itemIdx = 0; itemIdx < itemCount; )
    {
        int itemRemaining = itemCount - itemIdx;
        int itemsToScan;
        if (itemIdx == 0) // First iteration.
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
            await itemFunc(itemIdx, currentItemPos);

            currentItemPos.Y += itemLineHeightPx;
            itemIdx += 1;
        }

        currentItemPos.Y -= itemPerScroll * itemLineHeightPx;
        dofusWindow.MouseScroll(currentItemPos, -1);
        await Task.Delay(TimeSpan.FromMilliseconds(200));
    }
}

void KillAll(string processName)
{
    foreach (var dofusProcess in Process.GetProcessesByName(processName))
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

IEnumerable<ItemPrice> ReadServersAverageItemPrices(DofusData data)
{
    var logger = LoggerProvider.CreateLogger<Program>();

    string itemAveragePricesPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "Dofus", "itemAveragePrices.dat");
    if (!File.Exists(itemAveragePricesPath))
    {
        logger.LogWarning($"File \"{itemAveragePricesPath}\" does not exist");
        yield break;
    }

    var reader = new AmfReader(File.ReadAllBytes(itemAveragePricesPath));
    var root = (Amf3Object)reader.ReadAmf3();
    foreach (KeyValuePair<string, object> server in root.DynamicMembersAndValues)
    {
        string serverName = server.Key;
        if (!TryGetServerIdFromName(serverName, data, out int serverId))
        {
            continue;
        }

        var serverData = ((Amf3Object)server.Value).DynamicMembersAndValues;
        var lastUpdate = (DateTime)serverData["lastUpdate"];
        var itemPrices = (Dictionary<object, object>)serverData["items"];

        if (DateTime.Now - lastUpdate > TimeSpan.FromDays(1))
        {
            logger.LogWarning($"Ignoring server '{serverName}' with a too old last update '{lastUpdate}'");
            continue;
        }

        foreach (var itemPrice in itemPrices)
        {
            int itemId = int.Parse((string)itemPrice.Key);
            long price = itemPrice.Value switch
            {
                int p => p,
                double p => (long)p,
                _ => throw new Exception($"Unexpected type for price: {itemPrice.Value.GetType()}")
            };

            yield return new ItemPrice(serverId, itemId, null, price);
        }

        logger.LogInformation($"server: '{serverName}', last update: {lastUpdate}, item prices: {itemPrices.Count}");
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

void InitializeLogging()
{
    string? name = Environment.GetEnvironmentVariable("DOFUS_MARKET_LOGS_BASIC_AUTH_NAME");
    string? password = Environment.GetEnvironmentVariable("DOFUS_MARKET_LOGS_BASIC_AUTH_PASSWORD");

    var serilogConfiguration = new LoggerConfiguration()
        .MinimumLevel.Debug()
        .WriteTo.Console();

    if (name != null && password != null)
    {
        serilogConfiguration
            .WriteTo.GrafanaLoki(
                uri: "https://logs.dofus-market.com",
                labels: new [] { new LokiLabel { Key = "app", Value = "dofus-market-bot" } },
                credentials: new LokiCredentials { Login = name, Password = password });
    }

    Log.Logger = serilogConfiguration.CreateLogger();

    var loggerFactory = LoggerFactory.Create(builder =>
    {
        builder
            .AddSerilog(dispose: true)
            .SetMinimumLevel(LogLevel.Debug);
    });
    LoggerProvider.Initialize(loggerFactory);
}

record ItemPrice(int ServerId, int ItemId, int? Quantity, long Price);