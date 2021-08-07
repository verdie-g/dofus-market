using Dofus.Messages;
using Dofus.Serialization;

namespace Dofus.Types
{
    public class CharacterHardcoreOrEpicInformations : CharacterBaseInformations, INetworkMessage
    {
        internal new static int MessageId => 9632;

        public byte DeathState { get; private set; }
        public ushort DeathCount { get; private set; }
        public ushort DeathMaxLevel { get; private set; }

        public new void Serialize(DofusBinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public new void Deserialize(DofusBinaryReader reader)
        {
            base.Deserialize(reader);
            DeathState = reader.ReadByte();
            DeathCount = (ushort)reader.Read7BitEncodedInt();
            DeathMaxLevel = (ushort)reader.Read7BitEncodedInt();
        }
    }
}
