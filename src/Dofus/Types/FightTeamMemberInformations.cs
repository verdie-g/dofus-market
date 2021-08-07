using Dofus.Messages;
using Dofus.Serialization;

namespace Dofus.Types
{
    public class FightTeamMemberInformations : INetworkMessage
    {
        internal static int MessageId => 7292;

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
