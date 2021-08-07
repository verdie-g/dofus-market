using System;
using Dofus.Messages;
using Dofus.Serialization;

namespace Dofus.Types
{
    public class GroupMonsterStaticInformations : INetworkMessage
    {
        internal static int MessageId => 954;

        public MonsterInGroupLightInformations MainCreatureLightInfos { get; private set; } = new();
        public MonsterInGroupInformations[] Underlings { get; private set; } = Array.Empty<MonsterInGroupInformations>();

        public void Serialize(DofusBinaryWriter writer)
        {
            throw new NotImplementedException();
        }

        public void Deserialize(DofusBinaryReader reader)
        {
            MainCreatureLightInfos = reader.ReadObject<MonsterInGroupLightInformations>();
            Underlings = reader.ReadObjectCollection<MonsterInGroupInformations>();
        }
    }
}
