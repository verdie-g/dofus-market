using System;
using Dofus.Messages;
using Dofus.Serialization;

namespace Dofus.Types
{
    public class GroupMonsterStaticInformationsWithAlternatives : GroupMonsterStaticInformations, INetworkMessage
    {
        internal new static ushort MessageId => 822;

        public AlternativeMonsterInGroupLightInformations[] Alternatives { get; private set; } = Array.Empty<AlternativeMonsterInGroupLightInformations>();

        public new void Serialize(DofusBinaryWriter writer)
        {
            throw new NotImplementedException();
        }

        public new void Deserialize(DofusBinaryReader reader)
        {
            base.Deserialize(reader);
            Alternatives = reader.ReadObjectCollection<AlternativeMonsterInGroupLightInformations>();
        }
    }
}
