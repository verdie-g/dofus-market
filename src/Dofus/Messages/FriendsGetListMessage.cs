﻿using Dofus.Serialization;

namespace Dofus.Messages
{
    public class FriendsGetListMessage : INetworkMessage
    {
        internal static ushort MessageId => 3127;

        public void Serialize(DofusBinaryWriter writer)
        {
        }

        public void Deserialize(DofusBinaryReader reader)
        {
        }
    }
}
