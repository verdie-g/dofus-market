﻿using Dofus.Serialization;

namespace Dofus.Messages
{
    public class AnomalySubareaInformationRequestMessage : INetworkMessage
    {
        internal static int MessageId => 2749;

        public void Serialize(DofusBinaryWriter writer)
        {
        }

        public void Deserialize(DofusBinaryReader reader)
        {
            throw new System.NotImplementedException();
        }
    }
}
