using Dofus.Messages;
using Dofus.Serialization;

namespace Dofus.Types
{
    public class AbstractCharacterInformation : INetworkMessage
    {
        internal static int MessageId => 8427;

        public ulong Id { get; private set; }

        public void Serialize(DofusBinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public void Deserialize(DofusBinaryReader reader)
        {
            Id = (ulong)reader.Read7BitEncodedInt64();
        }
    }
}
