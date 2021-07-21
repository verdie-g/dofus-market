using System;
using Dofus.Messages;
using Dofus.Serialization;

namespace Dofus.Types
{
    public class EntityLook : INetworkMessage
    {
        internal static int MessageId => 9118;

        public ushort BonesId { get; private set; }
        public ushort[] Skins { get; private set; } = Array.Empty<ushort>();
        public int[] IndexedColors { get; private set; } = Array.Empty<int>();
        public short[] Scales { get; private set; } = Array.Empty<short>();
        public SubEntity[] SubEntities { get; private set; } = Array.Empty<SubEntity>();

        public void Serialize(DofusBinaryWriter writer)
        {
            throw new NotImplementedException();
        }

        public void Deserialize(DofusBinaryReader reader)
        {
            BonesId = (ushort)reader.Read7BitEncodedInt();
            Skins = reader.ReadCollection(r => (ushort)r.Read7BitEncodedInt());
            IndexedColors = reader.ReadCollection(r => r.ReadInt32());
            Scales = reader.ReadCollection(r => (short)r.Read7BitEncodedInt());
            SubEntities = reader.ReadObjectCollection<SubEntity>();
        }
    }
}
