using Dofus.Messages;
using Dofus.Serialization;

namespace Dofus.Types
{
    public class MonsterInGroupLightInformations : INetworkMessage
    {
        internal static ushort MessageId => 3401;

        public int GenericId { get; private set; }
        public byte Grade { get; private set; }
        public short Level { get; private set; }

        public void Serialize(DofusBinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public void Deserialize(DofusBinaryReader reader)
        {
            GenericId = reader.ReadInt32();
            Grade = reader.ReadByte();
            Level = reader.ReadInt16();
        }
    }
}
