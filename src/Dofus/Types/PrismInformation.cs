using Dofus.Messages;
using Dofus.Serialization;

namespace Dofus.Types
{
    public class PrismInformation : INetworkMessage
    {
        internal static int MessageId => 1753;

        public byte TypeId { get; private set; }
        public byte State { get; private set; }
        public int NextVulnerabilityDate { get; private set; }
        public int PlacementDate { get; private set; }
        public uint RewardTokenCount { get; private set; }

        public void Serialize(DofusBinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public void Deserialize(DofusBinaryReader reader)
        {
            TypeId = reader.ReadByte();
            State = reader.ReadByte();
            NextVulnerabilityDate = reader.ReadInt32();
            PlacementDate = reader.ReadInt32();
            RewardTokenCount = (uint)reader.Read7BitEncodedInt();
        }
    }
}
