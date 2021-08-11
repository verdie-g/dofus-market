using Dofus.Messages;
using Dofus.Serialization;

namespace Dofus.Types
{
    public class Item : INetworkMessage
    {
        internal static ushort MessageId => 4071;

        public void Serialize(DofusBinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public void Deserialize(DofusBinaryReader reader)
        {
        }
    }
}
