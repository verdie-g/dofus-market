using Dofus.Serialization;

namespace Dofus.Messages
{
    public class QuestListRequestMessage : INetworkMessage
    {
        internal static ushort MessageId => 5830;

        public void Serialize(DofusBinaryWriter writer)
        {
        }

        public void Deserialize(DofusBinaryReader reader)
        {
            throw new System.NotImplementedException();
        }
    }
}
