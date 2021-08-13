using System;
using Dofus.Messages;
using Dofus.Serialization;

namespace Dofus.Types
{
    public class HumanOptionFollower : HumanOption, INetworkMessage
    {
        internal new static ushort MessageId => 2314;

        public IndexedEntityLook[] FollowingCharactersLook { get; private set; } = Array.Empty<IndexedEntityLook>();

        public new void Serialize(DofusBinaryWriter writer)
        {
            throw new NotImplementedException();
        }

        public new void Deserialize(DofusBinaryReader reader)
        {
            base.Deserialize(reader);
            FollowingCharactersLook = reader.ReadObjectCollection<IndexedEntityLook>();
        }
    }
}
