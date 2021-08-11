using Dofus.Serialization;

namespace Dofus.Messages
{
    public class LeaveDialogRequestMessage : INetworkMessage
    {
        internal static ushort MessageId => 8404;

        public void Serialize(DofusBinaryWriter writer)
        {
        }

        public void Deserialize(DofusBinaryReader reader)
        {
            throw new System.NotImplementedException();
        }
    }
}
