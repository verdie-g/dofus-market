using Dofus.Messages;
using Dofus.Serialization;

namespace Dofus.Types
{
    public class ObjectEffectDice : ObjectEffect, INetworkMessage
    {
        internal new static ushort MessageId => 7506;

        public uint DiceNum { get; private set; }
        public uint DiceSide { get; private set; }
        public uint DiceConst { get; private set; }

        public new void Serialize(DofusBinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public new void Deserialize(DofusBinaryReader reader)
        {
            base.Deserialize(reader);
            DiceNum = (uint)reader.Read7BitEncodedInt();
            DiceSide = (uint)reader.Read7BitEncodedInt();
            DiceConst = (uint)reader.Read7BitEncodedInt();
        }
    }
}
