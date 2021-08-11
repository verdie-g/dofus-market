using Dofus.Serialization;

namespace Dofus.Messages
{
    public class SpouseGetInformationsMessage : INetworkMessage
    {
        internal static ushort MessageId => 2834;

        public void Serialize(DofusBinaryWriter writer)
        {
        }

        public void Deserialize(DofusBinaryReader reader)
        {
        }
    }
}
