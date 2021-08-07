using Dofus.Messages;
using Dofus.Serialization;

namespace Dofus.Types
{
    public class SubEntity : INetworkMessage
    {
        internal static int MessageId => 3806;

        public byte BindingPointCategory { get; private set; }
        public byte BindingPointIndex { get; private set; }
        public EntityLook SubEntityLook { get; private set; } = new();

        public void Serialize(DofusBinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public void Deserialize(DofusBinaryReader reader)
        {
            BindingPointCategory = reader.ReadByte();
            BindingPointIndex = reader.ReadByte();
            SubEntityLook.Deserialize(reader);
        }
    }
}
