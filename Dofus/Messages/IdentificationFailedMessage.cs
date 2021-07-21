using System;
using Dofus.Serialization;
using Dofus.Types;

namespace Dofus.Messages
{
    public class IdentificationFailedMessage : INetworkMessage
    {
        internal static int MessageId => 5778;
        public IdentificationFailureReason Reason { get; private set; }

        public void Serialize(DofusBinaryWriter writer) => throw new NotImplementedException();

        public void Deserialize(DofusBinaryReader reader)
        {
            Reason = (IdentificationFailureReason)reader.ReadByte();
        }
    }
}
