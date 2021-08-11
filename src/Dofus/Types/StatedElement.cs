using Dofus.Messages;
using Dofus.Serialization;

namespace Dofus.Types
{
    public class StatedElement : INetworkMessage
    {
        internal static ushort MessageId => 8521;

        public int ElementId { get; private set; }
        public ushort ElementCellId { get; private set; }
        public int ElementState { get; private set; }
        public bool OnCurrentMap { get; private set; }

        public void Serialize(DofusBinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public void Deserialize(DofusBinaryReader reader)
        {
            ElementId = reader.ReadInt32();
            ElementCellId = (ushort)reader.Read7BitEncodedInt();
            ElementState = reader.Read7BitEncodedInt();
            OnCurrentMap = reader.ReadBoolean();
        }
    }
}
