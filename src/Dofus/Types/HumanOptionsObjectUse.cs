using Dofus.Messages;
using Dofus.Serialization;

namespace Dofus.Types
{
    public class HumanOptionsObjectUse : HumanOption, INetworkMessage
    {
        internal new static ushort MessageId => 8293;

        public byte DelayTypeId { get; private set; }
        public double DelayEndTime { get; private set; }
        public ushort ObjectGid { get; private set; }

        public new void Serialize(DofusBinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public new void Deserialize(DofusBinaryReader reader)
        {
            base.Deserialize(reader);
            DelayTypeId = reader.ReadByte();
            DelayEndTime = reader.ReadDouble();
            ObjectGid = (ushort)reader.Read7BitEncodedInt();
        }
    }
}
