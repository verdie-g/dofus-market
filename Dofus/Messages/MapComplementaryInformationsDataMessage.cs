using System;
using Dofus.Serialization;
using Dofus.Types;

namespace Dofus.Messages
{
    public class MapComplementaryInformationsDataMessage : INetworkMessage
    {
        internal static int MessageId => 7028;

        public ushort SubAreaId { get; private set; }
        public long MapId { get; private set; }
        public HouseInformations[] Houses { get; private set; } = Array.Empty<HouseInformations>();
        public GameRolePlayActorInformations[] Actors { get; private set; } = Array.Empty<GameRolePlayActorInformations>();
        public InteractiveElement[] InteractiveElements { get; private set; } = Array.Empty<InteractiveElement>();
        public StatedElement[] StatedElements { get; private set; } = Array.Empty<StatedElement>();
        public MapObstacle[] Obstacles { get; private set; } = Array.Empty<MapObstacle>();
        public FightCommonInformations[] Fights { get; private set; } = Array.Empty<FightCommonInformations>();
        public bool HasAggressiveMonsters { get; private set; }
        public FightStartingPositions FightStartPositions { get; private set; } = new();

        public void Serialize(DofusBinaryWriter writer)
        {
            throw new NotImplementedException();
        }

        public void Deserialize(DofusBinaryReader reader)
        {
            SubAreaId = (ushort)reader.Read7BitEncodedInt();
            MapId = reader.ReadInt64();
            Houses = reader.ReadObjectCollection<HouseInformations>(true);
            Actors = reader.ReadObjectCollection<GameRolePlayActorInformations>(true);
            InteractiveElements = reader.ReadObjectCollection<InteractiveElement>(true);
            StatedElements = reader.ReadObjectCollection<StatedElement>();
            Obstacles = reader.ReadObjectCollection<MapObstacle>();
            Fights = reader.ReadObjectCollection<FightCommonInformations>();
            HasAggressiveMonsters = reader.ReadBoolean();
            FightStartPositions = reader.ReadObject<FightStartingPositions>();
        }
    }
}
