using Dofus.Serialization;

namespace Dofus.Messages
{
    public class AuthenticationTicketRefusedMessage : INetworkMessage
    {
        internal static ushort MessageId => 3932;

        public void Serialize(DofusBinaryWriter writer)
        {
        }

        public void Deserialize(DofusBinaryReader reader)
        {
        }
    }
}
