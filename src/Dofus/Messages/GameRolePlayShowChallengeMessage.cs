using Dofus.Serialization;
using Dofus.Types;

namespace Dofus.Messages
{
    public class GameRolePlayShowChallengeMessage : INetworkMessage
    {
        internal static ushort MessageId => 913;

        public FightCommonInformations CommonInfos { get; private set; } = new();

        public void Serialize(DofusBinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public void Deserialize(DofusBinaryReader reader)
        {
            CommonInfos = reader.ReadObject<FightCommonInformations>();
        }
    }
}
