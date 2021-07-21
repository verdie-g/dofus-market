using Dofus.Messages;
using Dofus.Serialization;

namespace Dofus.Types
{
    public class BasicGuildInformations : AbstractSocialGroupInfos, INetworkMessage
    {
        internal new static int MessageId => 5973;

        public uint GuildId { get; private set; }
        public string GuildName { get; private set; } = string.Empty;
        public byte GuildLevel { get; private set; }

        public new void Serialize(DofusBinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public new void Deserialize(DofusBinaryReader reader)
        {
            base.Deserialize(reader);
            GuildId = (uint)reader.Read7BitEncodedInt();
            GuildName = reader.ReadString();
            GuildLevel = reader.ReadByte();
        }
    }
}
