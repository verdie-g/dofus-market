using Dofus.Messages;
using Dofus.Serialization;

namespace Dofus.Types
{
    public class ObjectEffectDuration : ObjectEffect, INetworkMessage
    {
        internal new static int MessageId => 1342;

        public ushort Days { get; private set; }
        public byte Hours { get; private set; }
        public byte Minutes { get; private set; }

        public new void Serialize(DofusBinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public new void Deserialize(DofusBinaryReader reader)
        {
            base.Deserialize(reader);
            Days = (ushort)reader.Read7BitEncodedInt();
            Hours = reader.ReadByte();
            Minutes = reader.ReadByte();
        }
    }
}
