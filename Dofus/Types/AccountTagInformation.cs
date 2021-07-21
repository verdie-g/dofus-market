using System;
using Dofus.Messages;
using Dofus.Serialization;

namespace Dofus.Types
{
    public class AccountTagInformation : INetworkMessage
    {
        internal static int MessageId => 4827;
        public string Nickname { get; private set; } = string.Empty;
        public string TagNumber { get; private set; } = string.Empty;

        public void Serialize(DofusBinaryWriter writer) => throw new NotImplementedException();

        public void Deserialize(DofusBinaryReader reader)
        {
            Nickname = reader.ReadString();
            TagNumber = reader.ReadString();
        }
    }
}
