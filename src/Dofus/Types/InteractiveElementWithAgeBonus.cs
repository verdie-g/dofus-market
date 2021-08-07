using Dofus.Messages;
using Dofus.Serialization;

namespace Dofus.Types
{
    public class InteractiveElementWithAgeBonus : InteractiveElement, INetworkMessage
    {
        internal new static int MessageId => 7651;

        public short AgeBonus { get; private set; }

        public new void Serialize(DofusBinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public new void Deserialize(DofusBinaryReader reader)
        {
            base.Deserialize(reader);
            AgeBonus = reader.ReadInt16();
        }
    }
}
