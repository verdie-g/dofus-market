using Dofus.Serialization;

namespace Dofus.Messages
{
    public class BasicLatencyStatsRequestMessage : INetworkMessage
    {
        internal static int MessageId => 7797;

        public void Serialize(DofusBinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public void Deserialize(DofusBinaryReader reader)
        {
        }
    }
}
