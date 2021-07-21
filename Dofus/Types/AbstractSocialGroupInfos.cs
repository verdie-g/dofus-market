using Dofus.Messages;
using Dofus.Serialization;

namespace Dofus.Types
{
    public class AbstractSocialGroupInfos : INetworkMessage
    {
        internal static int MessageId => 6558;

        public void Serialize(DofusBinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public void Deserialize(DofusBinaryReader reader)
        {
        }
    }
}
