using Dofus.Messages;
using Dofus.Serialization;

namespace Dofus.Types
{
    public class GameContextActorInformations : GameContextActorPositionInformations, INetworkMessage
    {
        internal new static ushort MessageId => 3338;

        public EntityLook Look { get; private set; } = new();

        public new void Serialize(DofusBinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public new void Deserialize(DofusBinaryReader reader)
        {
            base.Deserialize(reader);
            Look.Deserialize(reader);
        }
    }
}
