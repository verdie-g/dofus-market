using Dofus.Serialization;

namespace Dofus.Messages
{
    public class CharacterSelectionMessage : INetworkMessage
    {
        internal static ushort MessageId => 7730;

        public ulong Id { get; init; }

        public void Serialize(DofusBinaryWriter writer)
        {
            writer.Write7BitEncodedInt64((long)Id);
        }

        public void Deserialize(DofusBinaryReader reader)
        {
            throw new System.NotImplementedException();
        }
    }
}
