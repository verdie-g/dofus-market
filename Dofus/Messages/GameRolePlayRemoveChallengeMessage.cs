using Dofus.Serialization;

namespace Dofus.Messages
{
    public class GameRolePlayRemoveChallengeMessage : INetworkMessage
    {
        internal static int MessageId => 7713;

        public ushort FightId { get; private set; }

        public void Serialize(DofusBinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public void Deserialize(DofusBinaryReader reader)
        {
            FightId = (ushort)reader.Read7BitEncodedInt();
        }
    }
}
