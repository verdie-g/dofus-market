using Dofus.Messages;
using Dofus.Serialization;

namespace Dofus.Types
{
    public class HumanOptionAlliance : HumanOption, INetworkMessage
    {
        internal new static int MessageId => 1332;

        public AllianceInformations AllianceInformations { get; private set; } = new();
        public byte Aggressable { get; private set; }

        public new void Serialize(DofusBinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public new void Deserialize(DofusBinaryReader reader)
        {
            base.Deserialize(reader);
            AllianceInformations = reader.ReadObject<AllianceInformations>();
            Aggressable = reader.ReadByte();
        }
    }
}
