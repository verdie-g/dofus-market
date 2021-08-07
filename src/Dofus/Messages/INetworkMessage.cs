using Dofus.Serialization;

namespace Dofus.Messages
{
    public interface INetworkMessage
    {
        void Serialize(DofusBinaryWriter writer);
        void Deserialize(DofusBinaryReader reader);
    }
}
