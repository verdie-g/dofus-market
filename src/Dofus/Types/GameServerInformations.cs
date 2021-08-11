using Dofus.Messages;
using Dofus.Serialization;

namespace Dofus.Types
{
    public class GameServerInformations : INetworkMessage
    {
        internal static ushort MessageId => 920;

        public bool IsMonoAccount { get; private set; }
        public bool IsSelectable { get; private set; }
        public short Id { get; private set; }
        public byte Type { get; private set; }
        public byte Status { get; private set; }
        public byte Completion { get; private set; }
        public byte CharactersCount { get; private set; }
        public byte CharactersSlots { get; private set; }
        public double Date { get; private set; }

        public void Serialize(DofusBinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public void Deserialize(DofusBinaryReader reader)
        {
            int flags = reader.ReadByte();
            IsMonoAccount = (flags & 0x1) != 0;
            IsSelectable = (flags & 0x2) != 0;
            Id = (short)reader.Read7BitEncodedInt();
            Type = reader.ReadByte();
            Status = reader.ReadByte();
            Completion = reader.ReadByte();
            CharactersCount = reader.ReadByte();
            CharactersSlots = reader.ReadByte();
            Date = reader.ReadDouble();
        }
    }
}
