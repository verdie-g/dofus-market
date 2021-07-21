using Dofus.Serialization;

namespace Dofus.Messages
{
    public class ClientKeyMessage : INetworkMessage
    {
        internal static int MessageId => 982;
        public string Key { get; init; } = string.Empty;

        public void Serialize(DofusBinaryWriter writer)
        {
            writer.Write(Key);
        }

        public void Deserialize(DofusBinaryReader reader) => throw new System.NotImplementedException();
    }
}
