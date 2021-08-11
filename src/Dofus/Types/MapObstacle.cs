using Dofus.Messages;
using Dofus.Serialization;

namespace Dofus.Types
{
    public class MapObstacle : INetworkMessage
    {
        internal static ushort MessageId => 7167;

        public ushort ObstacleCellId { get; private set; }
        public MapObstacleState State { get; private set; }

        public void Serialize(DofusBinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public void Deserialize(DofusBinaryReader reader)
        {
            ObstacleCellId = (ushort)reader.Read7BitEncodedInt();
            State = (MapObstacleState)reader.ReadByte();
        }
    }
}
