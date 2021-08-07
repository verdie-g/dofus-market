using System;
using Dofus.Messages;
using Dofus.Serialization;

namespace Dofus.Types
{
    public class PartyInvitationMemberInformations : CharacterBaseInformations, INetworkMessage
    {
        internal new static int MessageId => 6670;

        public short WorldX { get; private set; }
        public short WorldY { get; private set; }
        public long MapId { get; private set; }
        public ushort SubareaId { get; private set; }
        public PartyEntityBaseInformation[] Entities { get; private set; } = Array.Empty<PartyEntityBaseInformation>();

        public new void Serialize(DofusBinaryWriter writer)
        {
            throw new NotImplementedException();
        }

        public new void Deserialize(DofusBinaryReader reader)
        {
            base.Deserialize(reader);
            WorldX = reader.ReadInt16();
            WorldY = reader.ReadInt16();
            MapId = reader.ReadInt64();
            SubareaId = (ushort)reader.Read7BitEncodedInt();
            Entities = reader.ReadObjectCollection<PartyEntityBaseInformation>();
        }
    }
}
