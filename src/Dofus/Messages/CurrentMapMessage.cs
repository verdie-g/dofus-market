using Dofus.Serialization;

namespace Dofus.Messages
{
    public class CurrentMapMessage : INetworkMessage
    {
        internal static int MessageId => 3437;

        public long MapId { get; private set; }
        public string MapKey { get; private set; } = string.Empty;

        public void Serialize(DofusBinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public void Deserialize(DofusBinaryReader reader)
        {
            MapId = reader.ReadInt64();
            MapKey = reader.ReadString();
        }
    }
}
