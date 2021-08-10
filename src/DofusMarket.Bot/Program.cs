using DofusMarket.Bot.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Extensions.Logging;

namespace DofusMarket.Bot
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
                    services.AddSingleton(new DofusMetrics(
                        hostContext.Configuration["DofusMarket:Api:Host"],
                        hostContext.Configuration["DofusMarket:Api:Username"],
                        hostContext.Configuration["DofusMarket:Api:Password"],
                        new SerilogLoggerProvider().CreateLogger(typeof(DofusMetrics).FullName)));
                })
                .Build().Run();
        }
    }
}
