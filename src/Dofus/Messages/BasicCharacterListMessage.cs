using System;
using Dofus.Serialization;
using Dofus.Types;

namespace Dofus.Messages
{
    public class BasicCharacterListMessage : INetworkMessage
    {
        internal static int MessageId => 6379;

        public CharacterBaseInformations[] Characters { get; private set; } = Array.Empty<CharacterBaseInformations>();

        public void Serialize(DofusBinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public void Deserialize(DofusBinaryReader reader)
        {
            Characters = reader.ReadObjectCollection<CharacterBaseInformations>(true);
        }
    }
}
