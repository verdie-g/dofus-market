using Dofus.Messages;
using Dofus.Serialization;

namespace Dofus.Types
{
    public class FightTeamMemberCharacterInformations : FightTeamMemberInformations, INetworkMessage
    {
        internal new static ushort MessageId => 3311;

        public string Name { get; private set; } = string.Empty;
        public ushort Level { get; private set; }

        public new void Serialize(DofusBinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public new void Deserialize(DofusBinaryReader reader)
        {
            base.Deserialize(reader);
            Name = reader.ReadString();
            Level = (ushort)reader.Read7BitEncodedInt();
        }
    }
}
