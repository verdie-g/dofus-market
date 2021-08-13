using Dofus.Messages;
using Dofus.Serialization;

namespace Dofus.Types
{
    public class ObjectEffectMinMax : ObjectEffect, INetworkMessage
    {
        internal new static ushort MessageId => 6930;

        public uint Min { get; private set; }
        public uint Max { get; private set; }

        public new void Serialize(DofusBinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public new void Deserialize(DofusBinaryReader reader)
        {
            base.Deserialize(reader);
            Min = (uint)reader.Read7BitEncodedInt();
            Max = (uint)reader.Read7BitEncodedInt();
        }
    }
}
