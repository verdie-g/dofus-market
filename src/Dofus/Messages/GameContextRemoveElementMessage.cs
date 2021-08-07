using Dofus.Serialization;

namespace Dofus.Messages
{
    public class GameContextRemoveElementMessage : INetworkMessage
    {
        internal static int MessageId => 6263;

        public long Id { get; private set; }

        public void Serialize(DofusBinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public void Deserialize(DofusBinaryReader reader)
        {
            Id = reader.ReadInt64();
        }
    }
}
