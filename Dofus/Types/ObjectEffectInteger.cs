using Dofus.Messages;
using Dofus.Serialization;

namespace Dofus.Types
{
    public class ObjectEffectInteger : ObjectEffect, INetworkMessage
    {
        internal new static int MessageId => 624;

        public uint Value { get; private set; }

        public new void Serialize(DofusBinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public new void Deserialize(DofusBinaryReader reader)
        {
            base.Deserialize(reader);
            Value = (uint)reader.Read7BitEncodedInt();
        }
    }
}
