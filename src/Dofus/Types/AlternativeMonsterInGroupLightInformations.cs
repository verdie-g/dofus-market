using System;
using Dofus.Messages;
using Dofus.Serialization;

namespace Dofus.Types
{
    public class AlternativeMonsterInGroupLightInformations : INetworkMessage
    {
        internal static int MessageId => 6668;

        public int PlayerCount { get; private set; }
        public MonsterInGroupLightInformations[] Monsters { get; private set; } = Array.Empty<MonsterInGroupLightInformations>();

        public void Serialize(DofusBinaryWriter writer)
        {
            throw new NotImplementedException();
        }

        public void Deserialize(DofusBinaryReader reader)
        {
            PlayerCount = reader.ReadInt32();
            Monsters = reader.ReadObjectCollection<MonsterInGroupLightInformations>();
        }
    }
}
