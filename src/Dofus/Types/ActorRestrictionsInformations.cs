using Dofus.Messages;
using Dofus.Serialization;

namespace Dofus.Types
{
    public class ActorRestrictionsInformations : INetworkMessage
    {
        internal static int MessageId => 1254;

        public bool CantBeAggressed { get; private set; }
        public bool CantBeChallenged { get; private set; }
        public bool CantTrade { get; private set; }
        public bool CantBeAttackedByMutant { get; private set; }
        public bool CantRun { get; private set; }
        public bool ForceSlowWalk { get; private set; }
        public bool CantMinimize { get; private set; }
        public bool CantMove { get; private set; }
        public bool CantAggress { get; private set; }
        public bool CantChallenge { get; private set; }
        public bool CantExchange { get; private set; }
        public bool CantAttack { get; private set; }
        public bool CantChat { get; private set; }
        public bool CantBeMerchant { get; private set; }
        public bool CantUseObject { get; private set; }
        public bool CantUseTaxCollector { get; private set; }
        public bool CantUseInteractive { get; private set; }
        public bool CantSpeakToNpc { get; private set; }
        public bool CantChangeZone { get; private set; }
        public bool CantAttackMonster { get; private set; }
        public bool CantWalk8Directions { get; private set; }

        public void Serialize(DofusBinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public void Deserialize(DofusBinaryReader reader)
        {
            byte flags = reader.ReadByte();
            CantBeAggressed = (flags & 0x01) != 0;
            CantBeChallenged = (flags & 0x02) != 0;
            CantTrade = (flags & 0x04) != 0;
            CantBeAttackedByMutant = (flags & 0x08) != 0;
            CantRun = (flags & 0x10) != 0;
            ForceSlowWalk = (flags & 0x20) != 0;
            CantMinimize = (flags & 0x40) != 0;
            CantMove = (flags & 0x80) != 0;

            flags = reader.ReadByte();
            CantAggress = (flags & 0x01) != 0;
            CantChallenge = (flags & 0x02) != 0;
            CantExchange = (flags & 0x04) != 0;
            CantAttack = (flags & 0x08) != 0;
            CantChat = (flags & 0x10) != 0;
            CantBeMerchant = (flags & 0x20) != 0;
            CantUseObject = (flags & 0x40) != 0;
            CantUseTaxCollector = (flags & 0x80) != 0;

            flags = reader.ReadByte();
            CantUseInteractive = (flags & 0x01) != 0;
            CantSpeakToNpc = (flags & 0x02) != 0;
            CantChangeZone = (flags & 0x04) != 0;
            CantAttackMonster = (flags & 0x08) != 0;
            CantWalk8Directions = (flags & 0x10) != 0;
        }
    }
}
