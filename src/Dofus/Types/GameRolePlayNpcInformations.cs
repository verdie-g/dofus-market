using Dofus.Messages;
using Dofus.Serialization;

namespace Dofus.Types
{
    public class GameRolePlayNpcInformations : GameRolePlayActorInformations, INetworkMessage
    {
        internal new static ushort MessageId => 5203;

        public ushort NpcId { get; private set; }
        public bool Sex { get; private set; }
        public ushort SpecialArtworkId { get; private set; }

        public new void Serialize(DofusBinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public new void Deserialize(DofusBinaryReader reader)
        {
            base.Deserialize(reader);
            NpcId = (ushort)reader.Read7BitEncodedInt();
            Sex = reader.ReadBoolean();
            SpecialArtworkId = (ushort)reader.Read7BitEncodedInt();
        }
    }
}
