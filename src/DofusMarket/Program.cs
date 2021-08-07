using DofusMarket.Services;
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
                    services.AddSingleton(new DofusMetrics(hostContext.Configuration["PostgreSql:ConnectionString"]));
                })
                .Build().Run();
        }
    }
}
