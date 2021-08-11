using System;
using Dofus.Messages;
using Dofus.Serialization;

namespace Dofus.Types
{
    public class FightStartingPositions : INetworkMessage
    {
        internal static ushort MessageId => 2393;

        public ushort[] PositionsForChallengers { get; private set; } = Array.Empty<ushort>();
        public ushort[] PositionsForDefenders { get; private set; } = Array.Empty<ushort>();

        public void Serialize(DofusBinaryWriter writer)
        {
            throw new NotImplementedException();
        }

        public void Deserialize(DofusBinaryReader reader)
        {
            PositionsForChallengers = reader.ReadCollection(r => (ushort)r.Read7BitEncodedInt());
            PositionsForDefenders = reader.ReadCollection(r => (ushort)r.Read7BitEncodedInt());
        }
    }
}
