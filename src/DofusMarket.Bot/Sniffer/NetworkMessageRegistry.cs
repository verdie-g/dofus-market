using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace DofusMarket.Bot.Sniffer;

internal static class NetworkMessageRegistry
{
    private static readonly Dictionary<ushort, Type> MessageTypes = CollectNetworkMessageTypes();
    private static readonly Dictionary<Type, ushort> MessageIds = MessageTypes.ToDictionary(m => m.Value, m => m.Key);

    public static Type GetTypeFromId(ushort messageId)
    {
        if (TryGetTypeFromId(messageId, out Type? type))
        {
            return type;
        }

        throw new KeyNotFoundException($"No type found with id {messageId}");
    }

    public static bool TryGetTypeFromId(ushort messageId, [MaybeNullWhen(false)] out Type type)
    {
        return MessageTypes.TryGetValue(messageId, out type);
    }

    public static ushort GetIdFromType(Type type)
    {
        if (MessageIds.TryGetValue(type, out ushort typeId))
        {
            return typeId;
        }

        throw new KeyNotFoundException($"No type found for {type.Name}");
    }

    private static Dictionary<ushort, Type> CollectNetworkMessageTypes()
    {
        const string messageIdPropertyName = "MessageId";

        Dictionary<ushort, Type> messageTypes = new();
        foreach (Type messageType in typeof(NetworkMessageRegistry).Assembly.GetTypes())
        {
            if (messageType.GetInterface(nameof(INetworkMessage)) == null)
            {
                continue;
            }

            PropertyInfo? messageIdProperty = messageType
                .GetProperty(messageIdPropertyName, BindingFlags.Static | BindingFlags.NonPublic);
            if (messageIdProperty == null || messageIdProperty.PropertyType != typeof(ushort))
            {
                throw new Exception($"No 'internal static ushort {messageIdPropertyName}' found on type {messageType.Name}");
            }

            ushort messageId = (ushort)messageIdProperty.GetValue(null)!;
            if (messageTypes.TryGetValue(messageId, out Type? conflictingType))
            {
                throw new Exception($"Messages {messageType.Name} and {conflictingType.Name} have the same id ({messageId})");
            }

            messageTypes[messageId] = messageType;
        }

        return messageTypes;
    }
}