using Dofus.Serialization;

namespace Dofus.Messages
{
    public class AuthenticationTicketAcceptedMessage : INetworkMessage
    {
        internal static int MessageId => 8662;

        public void Serialize(DofusBinaryWriter writer)
        {
        }

        public void Deserialize(DofusBinaryReader reader)
        {
        }
    }
}
