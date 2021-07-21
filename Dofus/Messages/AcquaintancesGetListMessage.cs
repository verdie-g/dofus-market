using Dofus.Serialization;

namespace Dofus.Messages
{
    public class AcquaintancesGetListMessage : INetworkMessage
    {
        internal static int MessageId => 1167;

        public void Serialize(DofusBinaryWriter writer)
        {
        }

        public void Deserialize(DofusBinaryReader reader)
        {
        }
    }
}
