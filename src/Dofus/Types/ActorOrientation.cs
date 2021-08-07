using Dofus.Messages;
using Dofus.Serialization;

namespace Dofus.Types
{
    public class ActorOrientation : INetworkMessage
    {
        internal static int MessageId => 6675;

        public long Id { get; private set; }
        public byte Direction { get; private set; }

        public void Serialize(DofusBinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public void Deserialize(DofusBinaryReader reader)
        {
            Id = reader.ReadInt64();
            Direction = reader.ReadByte();
        }
    }
}
