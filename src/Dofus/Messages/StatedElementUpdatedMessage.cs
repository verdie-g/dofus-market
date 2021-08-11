using Dofus.Serialization;
using Dofus.Types;

namespace Dofus.Messages
{
    public class StatedElementUpdatedMessage : INetworkMessage
    {
        internal static ushort MessageId => 4006;

        public StatedElement StatedElement { get; private set; } = new();

        public void Serialize(DofusBinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public void Deserialize(DofusBinaryReader reader)
        {
            StatedElement = reader.ReadObject<StatedElement>();
        }
    }
}
