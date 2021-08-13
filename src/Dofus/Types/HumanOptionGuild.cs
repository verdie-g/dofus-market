using Dofus.Messages;
using Dofus.Serialization;

namespace Dofus.Types
{
    public class HumanOptionGuild : HumanOption, INetworkMessage
    {
        internal new static ushort MessageId => 7748;

        public GuildInformations GuildInformations { get; private set; } = new();

        public new void Serialize(DofusBinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public new void Deserialize(DofusBinaryReader reader)
        {
            base.Deserialize(reader);
            GuildInformations = reader.ReadObject<GuildInformations>();
        }
    }
}
