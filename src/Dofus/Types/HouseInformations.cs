using Dofus.Messages;
using Dofus.Serialization;

namespace Dofus.Types
{
    public class HouseInformations : INetworkMessage
    {
        internal static ushort MessageId => 3662;

        public int HouseId { get; private set; }
        public ushort ModelId { get; private set; }

        public void Serialize(DofusBinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public void Deserialize(DofusBinaryReader reader)
        {
            HouseId = reader.Read7BitEncodedInt();
            ModelId = (ushort)reader.Read7BitEncodedInt();
        }
    }
}
