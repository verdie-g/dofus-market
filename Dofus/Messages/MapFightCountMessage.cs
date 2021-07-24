using Dofus.Serialization;

namespace Dofus.Messages
{
    public class MapFightCountMessage : INetworkMessage
    {
        internal static int MessageId => 1367;

        public ushort FightCount { get; set; }

        public void Serialize(DofusBinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public void Deserialize(DofusBinaryReader reader)
        {
            FightCount = (ushort)reader.Read7BitEncodedInt();
        }
    }
}
