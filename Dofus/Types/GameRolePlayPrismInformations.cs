using Dofus.Messages;
using Dofus.Serialization;

namespace Dofus.Types
{
    public class GameRolePlayPrismInformations : GameRolePlayActorInformations, INetworkMessage
    {
        internal new static int MessageId => 7793;

        public PrismInformation Prism { get; private set; } = new();

        public new void Serialize(DofusBinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public new void Deserialize(DofusBinaryReader reader)
        {
            base.Deserialize(reader);
            Prism = reader.ReadObject<PrismInformation>(true);
        }
    }
}
