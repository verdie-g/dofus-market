using Dofus.Messages;
using Dofus.Serialization;

namespace Dofus.Types
{
    public class HumanOption : INetworkMessage
    {
        internal static int MessageId => 3247;

        public void Serialize(DofusBinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public void Deserialize(DofusBinaryReader reader)
        {
        }
    }
}
