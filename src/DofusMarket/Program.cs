using DofusMarket.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace DofusMarket
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureServices((hostContext, services) =>
                {
                    Log.Logger = new LoggerConfiguration()
                        .ReadFrom.Configuration(hostContext.Configuration)
                        .Enrich.FromLogContext()
                        .CreateLogger();

                    services.AddHostedService<Worker>();
                    services.AddSingleton<CryptoService>();
                    services.AddSingleton(new DofusMetrics(hostContext.Configuration["ConnectionStrings:DofusMarket"]));
                })
                .Build().Run();
        }
    }
}
