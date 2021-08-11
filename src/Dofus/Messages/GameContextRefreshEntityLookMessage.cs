using Dofus.Serialization;
using Dofus.Types;

namespace Dofus.Messages
{
    public class GameContextRefreshEntityLookMessage : INetworkMessage
    {
        internal static ushort MessageId => 6261;

        public long Id { get; private set; }
        public EntityLook Look { get; private set; } = new();

        public void Serialize(DofusBinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public void Deserialize(DofusBinaryReader reader)
        {
            Id = reader.ReadInt64();
            Look = reader.ReadObject<EntityLook>();
        }
    }
}
