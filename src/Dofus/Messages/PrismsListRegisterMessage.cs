using Dofus.Serialization;
using Dofus.Types;

namespace Dofus.Messages
{
    public class PrismsListRegisterMessage : INetworkMessage
    {
        internal static int MessageId => 4752;

        public PrismListen Listen { get; init; }

        public void Serialize(DofusBinaryWriter writer)
        {
            writer.Write((byte)Listen);
        }

        public void Deserialize(DofusBinaryReader reader)
        {
            throw new System.NotImplementedException();
        }
    }
}
