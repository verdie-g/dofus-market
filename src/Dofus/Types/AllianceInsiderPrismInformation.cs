using System;
using Dofus.Messages;
using Dofus.Serialization;

namespace Dofus.Types
{
    public class AllianceInsiderPrismInformation : PrismInformation, INetworkMessage
    {
        internal new static ushort MessageId => 9390;

        public int LastTimeSlotModificationDate { get; private set; }
        public uint LastTimeSlotModificationAuthorGuildId { get; private set; }
        public ulong LastTimeSlotModificationAuthorId { get; private set; }
        public string LastTimeSlotModificationAuthorName { get; private set; } = string.Empty;
        public ObjectItem[] ModulesObjects { get; private set; } = Array.Empty<ObjectItem>();

        public new void Serialize(DofusBinaryWriter writer)
        {
            throw new NotImplementedException();
        }

        public new void Deserialize(DofusBinaryReader reader)
        {
            base.Deserialize(reader);
            LastTimeSlotModificationDate = reader.ReadInt32();
            LastTimeSlotModificationAuthorGuildId = (uint)reader.Read7BitEncodedInt();
            LastTimeSlotModificationAuthorId = (ulong)reader.Read7BitEncodedInt64();
            LastTimeSlotModificationAuthorName = reader.ReadString();
            ModulesObjects = reader.ReadObjectCollection<ObjectItem>();
        }
    }
}
