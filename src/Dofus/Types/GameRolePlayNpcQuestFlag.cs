using System;
using Dofus.Messages;
using Dofus.Serialization;

namespace Dofus.Types
{
    public class GameRolePlayNpcQuestFlag : INetworkMessage
    {
        internal static int MessageId => 8580;

        public ushort[] QuestToValidId { get; private set; } = Array.Empty<ushort>();
        public ushort[] QuestToStartId { get; private set; } = Array.Empty<ushort>();

        public void Serialize(DofusBinaryWriter writer)
        {
            throw new NotImplementedException();
        }

        public void Deserialize(DofusBinaryReader reader)
        {
            QuestToValidId = reader.ReadCollection(r => (ushort)r.Read7BitEncodedInt());
            QuestToStartId = reader.ReadCollection(r => (ushort)r.Read7BitEncodedInt());
        }
    }
}
