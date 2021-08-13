using Dofus.Messages;
using Dofus.Serialization;

namespace Dofus.Types
{
    public class ObjectEffectDate : ObjectEffect, INetworkMessage
    {
        internal new static ushort MessageId => 6498;

        public uint Year { get; private set; }
        public byte Month { get; private set; }
        public byte Day { get; private set; }
        public byte Hour { get; private set; }
        public byte Minute { get; private set; }

        public new void Serialize(DofusBinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public new void Deserialize(DofusBinaryReader reader)
        {
            base.Deserialize(reader);
            Year = (ushort)reader.Read7BitEncodedInt();
            Month = reader.ReadByte();
            Day = reader.ReadByte();
            Hour = reader.ReadByte();
            Minute = reader.ReadByte();
        }
    }
}
