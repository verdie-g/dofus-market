using Dofus.Serialization;
using Dofus.Types;

namespace Dofus.Messages
{
    public class SetCharacterRestrictionMessage : INetworkMessage
    {
        internal static ushort MessageId => 4678;

        public long ActorId { get; set; }
        public ActorRestrictionsInformations Restrictions { get; private set; } = new();

        public void Serialize(DofusBinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public void Deserialize(DofusBinaryReader reader)
        {
            ActorId = reader.ReadInt64();
            Restrictions = reader.ReadObject<ActorRestrictionsInformations>();
        }
    }
}
