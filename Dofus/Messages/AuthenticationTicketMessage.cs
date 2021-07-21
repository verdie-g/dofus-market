using Dofus.Serialization;

namespace Dofus.Messages
{
    public class AuthenticationTicketMessage : INetworkMessage
    {
        internal static int MessageId => 3920;

        public string Lang { get; init; } = string.Empty;
        public string Ticket { get; init; } = string.Empty;

        public void Serialize(DofusBinaryWriter writer)
        {
            writer.Write(Lang);
            writer.Write(Ticket);
        }

        public void Deserialize(DofusBinaryReader reader)
        {
            throw new System.NotImplementedException();
        }
    }
}
