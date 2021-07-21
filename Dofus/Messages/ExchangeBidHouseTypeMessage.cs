using Dofus.Serialization;

namespace Dofus.Messages
{
    public class ExchangeBidHouseTypeMessage : INetworkMessage
    {
        internal static int MessageId => 1080;

        public uint Type { get; init; }
        public bool Follow { get; init; }

        public void Serialize(DofusBinaryWriter writer)
        {
            writer.Write7BitEncodedInt((int)Type);
            writer.Write(Follow);
        }

        public void Deserialize(DofusBinaryReader reader)
        {
            throw new System.NotImplementedException();
        }
    }
}
