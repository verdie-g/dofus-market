using System.Diagnostics;
using System.Threading.Channels;
using DofusMarket.Bot.Logging;
using Microsoft.Extensions.Logging;

namespace DofusMarket.Bot.Sniffer;

internal class NetworkMessageReader
{
    private static readonly ILogger Logger = LoggerProvider.CreateLogger<NetworkMessageReader>();

    private readonly ChannelReader<INetworkMessage> _messageChan;

    public NetworkMessageReader(DofusSniffer sniffer)
    {
        _messageChan = sniffer.Messages;
    }

    public async ValueTask<T> WaitForMessageAsync<T>(TimeSpan timeout = default) where T : INetworkMessage
    {
        var sw = Stopwatch.StartNew();
        using CancellationTokenSource cancellation = new(timeout == default ? TimeSpan.FromSeconds(30) : timeout);

        INetworkMessage message;
        try
        {
            do
            {
                message = await _messageChan.ReadAsync(cancellation.Token);
            } while (message is not T);
        }
        catch (OperationCanceledException)
        {
            Logger.LogWarning($"{nameof(NetworkMessageReader)}.{nameof(WaitForMessageAsync)}<{typeof(T).Name}>() -> {sw.ElapsedMilliseconds} ms (TIMEOUT)");
            throw;
        }

        Logger.LogDebug($"{nameof(NetworkMessageReader)}.{nameof(WaitForMessageAsync)}<{typeof(T).Name}>() -> {sw.ElapsedMilliseconds} ms");
        return (T)message;
    }
}