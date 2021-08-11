using Dofus.Serialization;

namespace Dofus.Messages
{
    public class SequenceNumberMessage : INetworkMessage
    {
        internal static ushort MessageId => 513;

        public ushort Number { get; init; }

        public void Serialize(DofusBinaryWriter writer)
        {
            writer.Write(Number);
        }

        public void Deserialize(DofusBinaryReader reader)
        {
            throw new System.NotImplementedException();
        }
    }
}
