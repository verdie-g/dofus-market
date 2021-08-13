using Dofus.Messages;
using Dofus.Serialization;

namespace Dofus.Types
{
    public class HumanOptionEmote : HumanOption, INetworkMessage
    {
        internal new static ushort MessageId => 5644;

        public int EmoteId { get; private set; }
        public double StartTime { get; private set; }

        public new void Serialize(DofusBinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public new void Deserialize(DofusBinaryReader reader)
        {
            base.Deserialize(reader);
            EmoteId = reader.ReadByte();
            StartTime = reader.ReadDouble();
        }
    }
}
