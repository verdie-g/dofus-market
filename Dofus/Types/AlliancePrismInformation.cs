using System;
using Dofus.Messages;
using Dofus.Serialization;

namespace Dofus.Types
{
    public class AlliancePrismInformation : PrismInformation, INetworkMessage
    {
        internal new static int MessageId => 4043;

        public AllianceInformations Alliance { get; private set; } = new();

        public new void Serialize(DofusBinaryWriter writer)
        {
            throw new NotImplementedException();
        }

        public new void Deserialize(DofusBinaryReader reader)
        {
            base.Deserialize(reader);
            Alliance = reader.ReadObject<AllianceInformations>();
        }
    }
}
