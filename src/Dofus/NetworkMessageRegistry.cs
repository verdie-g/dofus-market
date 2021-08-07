using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Dofus.Messages;

namespace Dofus
{
    internal static class NetworkMessageRegistry
    {
        private static readonly Dictionary<int, Type> MessageTypes = CollectNetworkMessageTypes();
        private static readonly Dictionary<Type, int> MessageIds = MessageTypes.ToDictionary(m => m.Value, m => m.Key);

        public static Type GetTypeFromId(int messageId)
        {
            if (TryGetTypeFromId(messageId, out Type? type))
            {
                return type;
            }

            throw new KeyNotFoundException($"No type found with id {messageId}");
        }

        public static bool TryGetTypeFromId(int messageId, [MaybeNullWhen(false)] out Type type)
        {
            return MessageTypes.TryGetValue(messageId, out type);
        }

        public static int GetIdFromType(Type type)
        {
            return MessageIds[type];
        }

        private static Dictionary<int, Type> CollectNetworkMessageTypes()
        {
            const string messageIdPropertyName = "MessageId";

            Dictionary<int, Type> messageTypes = new();
            foreach (Type messageType in typeof(NetworkMessageRegistry).Assembly.GetTypes())
            {
                if (messageType.GetInterface(nameof(INetworkMessage)) == null)
                {
                    continue;
                }

                PropertyInfo? messageIdProperty = messageType
                    .GetProperty(messageIdPropertyName, BindingFlags.Static | BindingFlags.NonPublic);
                if (messageIdProperty == null || messageIdProperty.PropertyType != typeof(int))
                {
                    throw new Exception($"No 'internal static int {messageIdPropertyName}' found on type {messageType.Name}");
                }

                int messageId = (int)messageIdProperty.GetValue(null)!;
                if (messageTypes.TryGetValue(messageId, out Type? conflictingType))
                {
                    throw new Exception($"Messages {messageType.Name} and {conflictingType.Name} have the same id ({messageId})");
                }

                messageTypes[messageId] = messageType;
            }

            return messageTypes;
        }
    }
}
