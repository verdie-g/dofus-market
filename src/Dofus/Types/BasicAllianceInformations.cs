using Dofus.Messages;
using Dofus.Serialization;

namespace Dofus.Types
{
    public class BasicAllianceInformations : AbstractSocialGroupInfos, INetworkMessage
    {
        internal new static ushort MessageId => 3126;

        public uint AllianceId { get; private set; }
        public string AllianceTag { get; private set; } = string.Empty;

        public new void Serialize(DofusBinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public new void Deserialize(DofusBinaryReader reader)
        {
            base.Deserialize(reader);
            AllianceId = (uint)reader.Read7BitEncodedInt();
            AllianceTag = reader.ReadString();
        }
    }
}
