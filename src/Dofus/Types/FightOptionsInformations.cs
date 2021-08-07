using Dofus.Messages;
using Dofus.Serialization;

namespace Dofus.Types
{
    public class FightOptionsInformations : INetworkMessage
    {
        internal static int MessageId => 4321;

        public bool IsSecret { get; private set; }
        public bool IsRestrictedToPartyOnly { get; private set; }
        public bool IsClosed { get; private set; }
        public bool IsAskingHelp { get; private set; }

        public void Serialize(DofusBinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public void Deserialize(DofusBinaryReader reader)
        {
            byte flags = reader.ReadByte();
            IsSecret = (flags & 0x1) != 0;
            IsRestrictedToPartyOnly = (flags & 0x2) != 0;
            IsClosed = (flags & 0x4) != 0;
            IsAskingHelp = (flags & 0x8) != 0;
        }
    }
}
