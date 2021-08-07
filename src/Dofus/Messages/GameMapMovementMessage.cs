using System;
using Dofus.Serialization;

namespace Dofus.Messages
{
    public class GameMapMovementMessage : INetworkMessage
    {
        internal static int MessageId => 4500;

        public short[] KeyMovements { get; private set; } = Array.Empty<short>();
        public short ForcedDirection { get; private set; }
        public long ActorId { get; private set; }

        public void Serialize(DofusBinaryWriter writer)
        {
            throw new NotImplementedException();
        }

        public void Deserialize(DofusBinaryReader reader)
        {
            KeyMovements = reader.ReadCollection(r => r.ReadInt16());
            ForcedDirection = reader.ReadInt16();
            ActorId = reader.ReadInt64();
        }
    }
}
