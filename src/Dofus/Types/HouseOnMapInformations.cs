using System;
using Dofus.Messages;
using Dofus.Serialization;

namespace Dofus.Types
{
    public class HouseOnMapInformations : HouseInformations, INetworkMessage
    {
        internal new static ushort MessageId => 2155;

        public int[] DoorsOnMap { get; private set; } = Array.Empty<int>();
        public HouseInstanceInformations[] HouseInstances { get; private set; } = Array.Empty<HouseInstanceInformations>();

        public new void Serialize(DofusBinaryWriter writer)
        {
            throw new NotImplementedException();
        }

        public new void Deserialize(DofusBinaryReader reader)
        {
            base.Deserialize(reader);
            DoorsOnMap = reader.ReadCollection(r => r.ReadInt32());
            HouseInstances = reader.ReadObjectCollection<HouseInstanceInformations>();
        }
    }
}
