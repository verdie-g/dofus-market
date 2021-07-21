using Dofus.Messages;
using Dofus.Serialization;

namespace Dofus.Types
{
    public class ObjectEffectMinMax : ObjectEffect, INetworkMessage
    {
        internal new static int MessageId => 6930;

        public uint MonsterCount { get; set; }

        public new void Serialize(DofusBinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public new void Deserialize(DofusBinaryReader reader)
        {
            base.Deserialize(reader);
            MonsterCount = (uint)reader.Read7BitEncodedInt();
        }
    }
}
