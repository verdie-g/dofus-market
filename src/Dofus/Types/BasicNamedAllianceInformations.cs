using Dofus.Messages;
using Dofus.Serialization;

namespace Dofus.Types
{
    public class BasicNamedAllianceInformations : BasicAllianceInformations, INetworkMessage
    {
        internal new static ushort MessageId => 56;

        public string AllianceName { get; private set; } = string.Empty;

        public new void Serialize(DofusBinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public new void Deserialize(DofusBinaryReader reader)
        {
            base.Deserialize(reader);
            AllianceName = reader.ReadString();
        }
    }
}
