using Dofus.Serialization;

namespace Dofus.Messages
{
    public class BasicNoOperationMessage : INetworkMessage
    {
        internal static ushort MessageId => 7386;

        public void Serialize(DofusBinaryWriter writer)
        {
        }

        public void Deserialize(DofusBinaryReader reader)
        {
        }
    }
}
