using Dofus.Messages;
using Dofus.Serialization;

namespace Dofus.Types
{
    public class GameContextActorPositionInformations : INetworkMessage
    {
        internal static ushort MessageId => 8832;

        public long ContextualId { get; private set; }
        public EntityDispositionInformations Disposition { get; private set; } = new();

        public void Serialize(DofusBinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public void Deserialize(DofusBinaryReader reader)
        {
            ContextualId = reader.ReadInt64();
            Disposition = reader.ReadObject<EntityDispositionInformations>(true);
        }
    }
}
