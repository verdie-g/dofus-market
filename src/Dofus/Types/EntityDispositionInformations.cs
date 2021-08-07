using Dofus.Messages;
using Dofus.Serialization;

namespace Dofus.Types
{
    public class EntityDispositionInformations : INetworkMessage
    {
        internal static int MessageId => 2799;

        public short CellId { get; private set; }
        public byte Direction { get; private set; }

        public void Serialize(DofusBinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public void Deserialize(DofusBinaryReader reader)
        {
            CellId = reader.ReadInt16();
            Direction = reader.ReadByte();
        }
    }
}
