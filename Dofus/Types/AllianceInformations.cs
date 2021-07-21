using Dofus.Messages;
using Dofus.Serialization;

namespace Dofus.Types
{
    public class AllianceInformations : BasicNamedAllianceInformations, INetworkMessage
    {
        internal new static int MessageId => 1398;

        public GuildEmblem AllianceEmblem { get; private set; } = new();

        public new void Serialize(DofusBinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public new void Deserialize(DofusBinaryReader reader)
        {
            base.Deserialize(reader);
            AllianceEmblem = reader.ReadObject<GuildEmblem>();
        }
    }
}
