using Dofus.Serialization;

namespace Dofus.Messages
{
    public class CredentialsAcknowledgementMessage : INetworkMessage
    {
        internal static int MessageId => 5417;

        public void Serialize(DofusBinaryWriter writer)
        {
        }

        public void Deserialize(DofusBinaryReader reader)
        {
        }
    }
}
