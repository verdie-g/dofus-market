﻿using Dofus.Serialization;

namespace Dofus.Messages
{
    public class IgnoredGetListMessage : INetworkMessage
    {
        internal static int MessageId => 2713;

        public void Serialize(DofusBinaryWriter writer)
        {
        }

        public void Deserialize(DofusBinaryReader reader)
        {
        }
    }
}
