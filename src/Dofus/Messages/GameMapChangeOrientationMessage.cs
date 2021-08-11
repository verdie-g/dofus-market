using Dofus.Serialization;
using Dofus.Types;

namespace Dofus.Messages
{
    public class GameMapChangeOrientationMessage : INetworkMessage
    {
        internal static ushort MessageId => 2457;

        public ActorOrientation Orientation { get; private set; } = new();

        public void Serialize(DofusBinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public void Deserialize(DofusBinaryReader reader)
        {
            Orientation = reader.ReadObject<ActorOrientation>();
        }
    }
}
