using Dofus.Serialization;

namespace Dofus.Messages
{
    public class CharacterListRequestMessage : INetworkMessage
    {
        internal static ushort MessageId => 4918;

        public void Serialize(DofusBinaryWriter writer)
        {
        }

        public void Deserialize(DofusBinaryReader reader)
        {
        }
    }
}
