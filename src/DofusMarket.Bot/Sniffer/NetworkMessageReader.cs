using DofusMarket.Bot.Logging;
using Microsoft.Extensions.Logging;

namespace DofusMarket.Bot.Sniffer;

internal class NetworkMessageReader
{
    private static readonly ILogger Logger = LoggerProvider.CreateLogger<NetworkMessageReader>();

    private readonly IAsyncEnumerator<INetworkMessage> _messageEnumerator;

    public NetworkMessageReader(DofusSniffer sniffer)
    {
        _messageEnumerator = sniffer.Messages.GetAsyncEnumerator();
    }

    public async ValueTask<T> WaitForMessageAsync<T>() where T : INetworkMessage
    {
        Logger.LogDebug($"{nameof(NetworkMessageReader)}.{nameof(WaitForMessageAsync)}<{typeof(T).Name}>()");

        using CancellationTokenSource cts = new(TimeSpan.FromSeconds(30));

        do
        {
            if (!await _messageEnumerator.MoveNextAsync().AsTask().WaitAsync(cts.Token))
            {
                throw new InvalidOperationException("No more messages");
            }
        } while (_messageEnumerator.Current is not T);

        return (T)_messageEnumerator.Current;
    }
}