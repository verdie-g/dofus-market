using System;
using Dofus.Serialization;
using Dofus.Types;

namespace Dofus.Messages
{
    public class CharacterSelectedSuccessMessage : INetworkMessage
    {
        internal static ushort MessageId => 7662;

        public CharacterBaseInformations Infos { get; private set; } = new();
        public bool IsCollectionStats { get; private set; }

        public void Serialize(DofusBinaryWriter writer)
        {
            throw new NotImplementedException();
        }

        public void Deserialize(DofusBinaryReader reader)
        {
            Infos.Deserialize(reader);
            IsCollectionStats = reader.ReadBoolean();
        }
    }
}
