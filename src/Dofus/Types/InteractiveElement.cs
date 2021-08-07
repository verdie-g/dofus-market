using System;
using Dofus.Messages;
using Dofus.Serialization;

namespace Dofus.Types
{
    public class InteractiveElement : INetworkMessage
    {
        internal static int MessageId => 8429;

        public int ElementId { get; private set; }
        public int ElementTypeId { get; private set; }
        public InteractiveElementSkill[] EnabledSkills { get; private set; } = Array.Empty<InteractiveElementSkill>();
        public InteractiveElementSkill[] DisabledSkills { get; private set; } = Array.Empty<InteractiveElementSkill>();
        public bool OnCurrentMap { get; private set; }

        public void Serialize(DofusBinaryWriter writer)
        {
            throw new NotImplementedException();
        }

        public void Deserialize(DofusBinaryReader reader)
        {
            ElementId = reader.ReadInt32();
            ElementTypeId = reader.ReadInt32();
            EnabledSkills = reader.ReadObjectCollection<InteractiveElementSkill>(true);
            DisabledSkills = reader.ReadObjectCollection<InteractiveElementSkill>(true);
            OnCurrentMap = reader.ReadBoolean();
        }
    }
}
