using Dofus.Messages;
using Dofus.Serialization;

namespace Dofus.Types
{
    public class HouseInstanceInformations : INetworkMessage
    {
        internal static ushort MessageId => 4670;

        public bool SecondHand { get; private set; }
        public bool IsLocked { get; private set; }
        public bool HasOwner { get; private set; }
        public int InstanceId { get; private set; }
        public AccountTagInformation OwnerTag { get; private set; } = new();
        public long Price { get; private set; }

        public void Serialize(DofusBinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public void Deserialize(DofusBinaryReader reader)
        {
            byte flags = reader.ReadByte();
            SecondHand = (flags & 0x1) != 0;
            IsLocked = (flags & 0x2) != 0;
            HasOwner = (flags & 0x4) != 0;

            InstanceId = reader.ReadInt32();
            OwnerTag = reader.ReadObject<AccountTagInformation>();
            Price = reader.Read7BitEncodedInt64();
        }
    }
}
