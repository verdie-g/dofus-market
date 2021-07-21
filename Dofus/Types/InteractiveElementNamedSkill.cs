using Dofus.Messages;
using Dofus.Serialization;

namespace Dofus.Types
{
    public class InteractiveElementNamedSkill : InteractiveElementSkill, INetworkMessage
    {
        internal new static int MessageId => 8215;

        public uint NameId { get; private set; }

        public new void Serialize(DofusBinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public new void Deserialize(DofusBinaryReader reader)
        {
            base.Deserialize(reader);
            NameId = (uint)reader.Read7BitEncodedInt();
        }
    }
}
