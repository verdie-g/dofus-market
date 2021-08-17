using Dofus.Serialization;

namespace Dofus.Messages
{
    public class CheckFileRequestMessage : INetworkMessage
    {
        internal static ushort MessageId => 7250;

        public string Filename { get; private set; } = string.Empty;
        public byte Type { get; private set; }

        public void Serialize(DofusBinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public void Deserialize(DofusBinaryReader reader)
        {
            Filename = reader.ReadString();
            Type = reader.ReadByte();
        }
    }
}
