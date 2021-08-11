using Dofus.Serialization;
using Dofus.Types;

namespace Dofus.Messages
{
    public class MapFightStartPositionsUpdateMessage : INetworkMessage
    {
        internal static ushort MessageId => 7470;

        public long MapId { get; private set; }
        public FightStartingPositions FightStartPositions { get; private set; } = new();

        public void Serialize(DofusBinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public void Deserialize(DofusBinaryReader reader)
        {
            MapId = reader.ReadInt64();
            FightStartPositions = reader.ReadObject<FightStartingPositions>();
        }
    }
}
