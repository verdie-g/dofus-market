﻿using Dofus.Serialization;

namespace Dofus.Messages
{
    public class SequenceNumberRequestMessage : INetworkMessage
    {
        internal static int MessageId => 3495;

        public void Serialize(DofusBinaryWriter writer)
        {
        }

        public void Deserialize(DofusBinaryReader reader)
        {
        }
    }
}
