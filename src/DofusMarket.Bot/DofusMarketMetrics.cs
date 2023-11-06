using System.Diagnostics.Metrics;
using System.Text;
using DofusMarket.Bot.DataReader;
using DofusMarket.Bot.Internationalization;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;

namespace DofusMarket.Bot;

internal class DofusMarketMetrics
{
    private static readonly Meter Meter = new(typeof(Program).Assembly.GetName().FullName);

    private static readonly MeterProvider MeterProvider = Sdk.CreateMeterProviderBuilder()
        .ConfigureResource(resource => resource.AddService("dofus-market-bot", autoGenerateServiceInstanceId: false))
        .AddMeter(Meter.Name)
        .SetMaxMetricPointsPerMetricStream(300_000)
        .AddOtlpExporter(options =>
        {
            options.Protocol = OtlpExportProtocol.HttpProtobuf;
            options.Endpoint = new Uri("https://metrics.dofus-market.com/otlp/v1/metrics");
            options.Headers = "Authorization=Basic " + BuildBasicAuthCredentials();
            options.TimeoutMilliseconds = 30_000;
        })
        .Build()!;

    private static readonly ObservableGauge<long> PriceGauge = Meter.CreateObservableGauge(
                name: "dofus.item.price",
                () => Interlocked.Exchange(ref _itemPriceMeasurements, new List<Measurement<long>>())!,
                unit: "{kamas}",
                description: "auction house price of an item");
    private static readonly ObservableGauge<long> AveragePriceGauge = Meter.CreateObservableGauge(
                name: "dofus.item.average_price",
                () => Interlocked.Exchange(ref _itemAveragePriceMeasurements, new List<Measurement<long>>())!,
                unit: "{kamas}",
                description: "average sell price of an item in the last 30 days");

    // Non-observable gauge are not available yet (https://github.com/dotnet/runtime/issues/92625) so the
    // measurements need to be stored.
    private static List<Measurement<long>> _itemPriceMeasurements = new();
    private static List<Measurement<long>> _itemAveragePriceMeasurements = new();

    private static string BuildBasicAuthCredentials()
    {
        string? name = Environment.GetEnvironmentVariable("DOFUS_MARKET_METRICS_BASIC_AUTH_NAME");
        string? password = Environment.GetEnvironmentVariable("DOFUS_MARKET_METRICS_BASIC_AUTH_PASSWORD");
        if (name == null || password == null)
        {
            throw new Exception("DOFUS_MARKET_METRICS_BASIC_AUTH_* variables were not set");
        }

        return Convert.ToBase64String(Encoding.ASCII.GetBytes(name + ':' + password));
    }

    private readonly DofusData _dofusData;

    public DofusMarketMetrics(DofusData dofusData)
    {
        _dofusData = dofusData;
    }

    public void WriteItemPrice(ItemPrice itemPrice)
    {
        _itemPriceMeasurements.Add(ItemPriceToMeasurement(itemPrice));
    }

    public void WriteItemAveragePrices(IEnumerable<ItemPrice> itemPrices)
    {
        var measurements = itemPrices.Select(ItemPriceToMeasurement).ToList();
        Interlocked.Exchange(ref _itemAveragePriceMeasurements, measurements);
    }

    private Measurement<long> ItemPriceToMeasurement(ItemPrice itemPrice)
    {
        var serversData = _dofusData.GetDataForType("Servers");
        var itemsData = _dofusData.GetDataForType("Items");
        var itemTypesData = _dofusData.GetDataForType("ItemTypes");

        int serverNameId = (int)serversData[itemPrice.ServerId]["nameId"]!;
        string serverNameFr = _dofusData.GetText(serverNameId, DofusLanguages.French);
        string serverNameCanonicalFr = _dofusData.GetUndiacriticText(serverNameId, DofusLanguages.French);

        int itemNameId = (int)itemsData[itemPrice.ItemId]["nameId"]!;
        string itemNameFr = _dofusData.GetText(itemNameId, DofusLanguages.French);
        string itemNameCanonicalFr = _dofusData.GetUndiacriticText(itemNameId, DofusLanguages.French);

        int itemLevel = (int)itemsData[itemPrice.ItemId]["level"]!;

        int itemTypeId = (int)itemsData[itemPrice.ItemId]["typeId"]!;
        int itemTypeNameId = (int)itemTypesData[itemTypeId]["nameId"]!;
        string itemTypeNameFr = _dofusData.GetText(itemTypeNameId, DofusLanguages.French);
        string itemTypeNameCanonicalFr = _dofusData.GetUndiacriticText(itemTypeNameId, DofusLanguages.French);

        var tags = new List<KeyValuePair<string, object?>>()
        {
            new("server.id", itemPrice.ServerId),
            new("server.name", serverNameFr),
            new("server.name_canonical", serverNameCanonicalFr),
            new("item.id", itemPrice.ItemId),
            new("item.name", itemNameFr),
            new("item.name_canonical", itemNameCanonicalFr),
            new("item.level", itemLevel),
            new("item_type.id", itemTypeId),
            new("item_type.name", itemTypeNameFr),
            new("item_type.name_canonical", itemTypeNameCanonicalFr),
            new("language", DofusLanguages.French),
        };

        if (itemPrice.Quantity.HasValue)
        {
            tags.Add(new KeyValuePair<string, object?>("item.quantity", itemPrice.Quantity.Value));
        }

        return new Measurement<long>(itemPrice.Price, tags);
    }
}