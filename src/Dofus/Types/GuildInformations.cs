using Dofus.Messages;
using Dofus.Serialization;

namespace Dofus.Types
{
    public class GuildInformations : BasicGuildInformations, INetworkMessage
    {
        internal new static int MessageId => 6465;

        public GuildEmblem GuildEmblem { get; private set; } = new();

        public new void Serialize(DofusBinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public new void Deserialize(DofusBinaryReader reader)
        {
            base.Deserialize(reader);
            GuildEmblem = reader.ReadObject<GuildEmblem>();
        }
    }
}
