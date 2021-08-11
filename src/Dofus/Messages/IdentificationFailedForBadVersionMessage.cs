using Dofus.Serialization;
using Dofus.Types;

namespace Dofus.Messages
{
    public class IdentificationFailedForBadVersionMessage : INetworkMessage
    {
        internal static ushort MessageId => 8989;

        public DofusVersion RequiredVersion { get; private set; } = new();

        public void Serialize(DofusBinaryWriter writer)
        {
            RequiredVersion.Serialize(writer);
        }

        public void Deserialize(DofusBinaryReader reader)
        {
            RequiredVersion.Deserialize(reader);
        }
    }
}
