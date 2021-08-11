using Dofus.Serialization;

namespace Dofus.Messages
{
    public class MapInformationsRequestMessage : INetworkMessage
    {
        internal static ushort MessageId => 4598;
        public long MapId { get; init; }

        public void Serialize(DofusBinaryWriter writer)
        {
            writer.Write(MapId);
        }

        public void Deserialize(DofusBinaryReader reader)
        {
            throw new System.NotImplementedException();
        }
    }
}
