using System.Diagnostics.Metrics;
using System.Text;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;

namespace DofusMarket.Bot;

internal static class DofusMarketMetrics
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
                                                      () => Interlocked.Exchange(ref _itemPriceMeasurements, Array.Empty<Measurement<long>>()),
                                                      unit: "{kamas}",
                                                      description: "auction house price of an item");

    private static readonly ObservableGauge<long> AveragePriceGauge = Meter.CreateObservableGauge(
        name: "dofus.item.average_price",
        () => Interlocked.Exchange(ref _itemAveragePriceMeasurements, Array.Empty<Measurement<long>>()),
        unit: "{kamas}",
        description: "average sell price of an item in the last 30 days");

    // Non-observable gauge are not available yet (https://github.com/dotnet/runtime/issues/92625) so the
    // measurements need to be stored.
    private static Measurement<long>[] _itemPriceMeasurements = Array.Empty<Measurement<long>>();
    private static Measurement<long>[] _itemAveragePriceMeasurements = Array.Empty<Measurement<long>>();

    public static void UpdateItemPriceMeasurements(Measurement<long>[] measurements)
    {
        Interlocked.Exchange(ref _itemPriceMeasurements, measurements);
    }

    public static void UpdateAverageItemPriceMeasurements(Measurement<long>[] measurements)
    {
        Interlocked.Exchange(ref _itemAveragePriceMeasurements, measurements);
    }

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
}