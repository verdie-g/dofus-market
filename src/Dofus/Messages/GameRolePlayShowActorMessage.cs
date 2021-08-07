using Dofus.Serialization;
using Dofus.Types;

namespace Dofus.Messages
{
    public class GameRolePlayShowActorMessage : INetworkMessage
    {
        internal static int MessageId => 2285;

        public GameRolePlayActorInformations Informations { get; set; } = new();

        public void Serialize(DofusBinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public void Deserialize(DofusBinaryReader reader)
        {
            Informations = reader.ReadObject<GameRolePlayActorInformations>(true);
        }
    }
}
