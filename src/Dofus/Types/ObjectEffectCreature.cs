using Dofus.Messages;
using Dofus.Serialization;

namespace Dofus.Types
{
    public class ObjectEffectCreature : ObjectEffect, INetworkMessage
    {
        internal new static int MessageId => 222;

        public uint MonsterFamilyId { get; private set; }

        public new void Serialize(DofusBinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public new void Deserialize(DofusBinaryReader reader)
        {
            base.Deserialize(reader);
            MonsterFamilyId = (ushort)reader.Read7BitEncodedInt();
        }
    }
}
