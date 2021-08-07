using System;
using Dofus.Serialization;

namespace Dofus.Messages
{
    public class SelectedServerDataMessage : INetworkMessage
    {
        internal static int MessageId => 1568;

        public short ServerId { get; private set; }
        public string Address { get; private set; } = string.Empty;
        public int[] Port { get; private set; } = Array.Empty<int>();
        public bool CanCreateNewCharacter { get; private set; }
        public byte[] Ticket { get; private set; } = Array.Empty<byte>();

        public void Serialize(DofusBinaryWriter writer)
        {
            throw new NotImplementedException();
        }

        public void Deserialize(DofusBinaryReader reader)
        {
            ServerId = (short)reader.Read7BitEncodedInt();
            Address = reader.ReadString();
            Port = reader.ReadCollection(r => r.Read7BitEncodedInt());
            CanCreateNewCharacter = reader.ReadBoolean();
            int ticketLength = reader.Read7BitEncodedInt();
            Ticket = reader.ReadBytes(ticketLength);
        }
    }
}
