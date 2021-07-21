using Dofus.Messages;
using Dofus.Serialization;

namespace Dofus.Types
{
    public class GuildEmblem : INetworkMessage
    {
        internal static int MessageId => 150;

        public ushort SymbolShape { get; private set; }
        public int SymbolColor { get; private set; }
        public byte BackgroundShape { get; private set; }
        public int BackgroundColor { get; private set; }

        public void Serialize(DofusBinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public void Deserialize(DofusBinaryReader reader)
        {
            SymbolShape = (ushort)reader.Read7BitEncodedInt();
            SymbolColor = reader.ReadInt32();
            BackgroundShape = reader.ReadByte();
            BackgroundColor = reader.ReadInt32();
        }
    }
}
