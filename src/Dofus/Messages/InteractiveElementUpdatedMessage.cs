using Dofus.Serialization;
using Dofus.Types;

namespace Dofus.Messages
{
    public class InteractiveElementUpdatedMessage : INetworkMessage
    {
        internal static int MessageId => 9844;

        public InteractiveElement InteractiveElement { get; private set; } = new();

        public void Serialize(DofusBinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public void Deserialize(DofusBinaryReader reader)
        {
            InteractiveElement = reader.ReadObject<InteractiveElement>();
        }
    }
}
