using DofusMarket.Services;
using InfluxDB.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DofusMarket
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();
                    services.AddSingleton<CryptoService>();
                    services.AddSingleton(new DofusMetrics(CreateInfluxDbClient(hostContext.Configuration)));
                })
                .Build().Run();
        }

        private static InfluxDBClient CreateInfluxDbClient(IConfiguration configuration)
        {
            var influxConf = configuration.GetSection("InfluxDb");
            return InfluxDBClientFactory.Create(InfluxDBClientOptions.Builder.CreateNew()
                .Url(influxConf["Url"])
                .AuthenticateToken(influxConf["Token"])
                .Org(influxConf["Organisation"])
                .Bucket(influxConf["Bucket"])
                .Build());
        }
    }
}
