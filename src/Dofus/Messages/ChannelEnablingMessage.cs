using Dofus.Serialization;

namespace Dofus.Messages
{
    public class ChannelEnablingMessage : INetworkMessage
    {
        internal static ushort MessageId => 5493;

        public byte Channel { get; init; }
        public bool Enable { get; init; }

        public void Serialize(DofusBinaryWriter writer)
        {
            writer.Write(Channel);
            writer.Write(Enable);
        }

        public void Deserialize(DofusBinaryReader reader)
        {
            throw new System.NotImplementedException();
        }
    }
}
