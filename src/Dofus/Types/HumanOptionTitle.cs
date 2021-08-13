using Dofus.Messages;
using Dofus.Serialization;

namespace Dofus.Types
{
    public class HumanOptionTitle : HumanOption, INetworkMessage
    {
        internal new static ushort MessageId => 5299;

        public ushort TitleId { get; private set; }
        public string TitleParam { get; private set; } = string.Empty;

        public new void Serialize(DofusBinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public new void Deserialize(DofusBinaryReader reader)
        {
            base.Deserialize(reader);
            TitleId = (ushort)reader.Read7BitEncodedInt();
            TitleParam = reader.ReadString();
        }
    }
}
