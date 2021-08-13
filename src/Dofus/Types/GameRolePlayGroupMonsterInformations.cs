using Dofus.Messages;
using Dofus.Serialization;

namespace Dofus.Types
{
    public class GameRolePlayGroupMonsterInformations : GameRolePlayActorInformations, INetworkMessage
    {
        internal new static ushort MessageId => 475;

        public bool KeyRingBonus { get; private set; }
        public bool HasHardcoreDrop { get; private set; }
        public bool HasAvaRewardToken { get; private set; }
        public GroupMonsterStaticInformations StaticInfos { get; private set; } = new();
        public byte LootShare { get; private set; }
        public byte AlignmentSide { get; private set; }

        public new void Serialize(DofusBinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public new void Deserialize(DofusBinaryReader reader)
        {
            base.Deserialize(reader);

            byte flags = reader.ReadByte();
            KeyRingBonus = (flags & 1) != 0;
            HasHardcoreDrop = (flags & 2) != 0;
            HasAvaRewardToken = (flags & 4) != 0;

            StaticInfos = reader.ReadObject<GroupMonsterStaticInformations>(true);
            LootShare = reader.ReadByte();
            AlignmentSide = reader.ReadByte();
        }
    }
}
