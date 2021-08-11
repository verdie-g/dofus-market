using System;
using Dofus.Messages;
using Dofus.Serialization;

namespace Dofus.Types
{
    public class HumanInformations : INetworkMessage
    {
        internal static ushort MessageId => 4572;

        public ActorRestrictionsInformations Restrictions { get; private set; } = new();
        public bool Sex { get; private set; }
        public HumanOption[] Options { get; private set; } = Array.Empty<HumanOption>();

        public void Serialize(DofusBinaryWriter writer)
        {
            throw new NotImplementedException();
        }

        public void Deserialize(DofusBinaryReader reader)
        {
            Restrictions = reader.ReadObject<ActorRestrictionsInformations>();
            Sex = reader.ReadBoolean();
            Options = reader.ReadObjectCollection<HumanOption>(true);
        }
    }
}
