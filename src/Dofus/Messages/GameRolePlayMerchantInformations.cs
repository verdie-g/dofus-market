using System;
using Dofus.Serialization;
using Dofus.Types;

namespace Dofus.Messages
{
    public class GameRolePlayMerchantInformations : GameRolePlayNamedActorInformations, INetworkMessage
    {
        internal new static ushort MessageId => 2992;

        public byte SellType { get; set; }
        public HumanOption[] Options { get; private set; } = Array.Empty<HumanOption>();

        public new void Serialize(DofusBinaryWriter writer)
        {
            throw new NotImplementedException();
        }

        public new void Deserialize(DofusBinaryReader reader)
        {
            base.Deserialize(reader);
            SellType = reader.ReadByte();
            Options = reader.ReadObjectCollection<HumanOption>(true);
        }
    }
}
