using Dofus.Serialization;

namespace Dofus.Messages
{
    public class GameContextCreateRequestMessage : INetworkMessage
    {
        internal static ushort MessageId => 8231;

        public void Serialize(DofusBinaryWriter writer)
        {
        }

        public void Deserialize(DofusBinaryReader reader)
        {
        }
    }
}
