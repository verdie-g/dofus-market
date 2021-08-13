using Dofus.Messages;
using Dofus.Serialization;

namespace Dofus.Types
{
    public class ObjectEffectString : ObjectEffect, INetworkMessage
    {
        internal new static ushort MessageId => 3066;

        public string Value { get; private set; } = string.Empty;

        public new void Serialize(DofusBinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public new void Deserialize(DofusBinaryReader reader)
        {
            base.Deserialize(reader);
            Value = reader.ReadString();
        }
    }
}
