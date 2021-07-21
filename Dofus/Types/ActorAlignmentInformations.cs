using Dofus.Messages;
using Dofus.Serialization;

namespace Dofus.Types
{
    public class ActorAlignmentInformations : INetworkMessage
    {
        internal static int MessageId => 927;

        public byte AlignmentSide { get; private set; }
        public byte AlignmentValue { get; private set; }
        public byte AlignmentGrade { get; private set; }
        public double CharacterPower { get; private set; }

        public void Serialize(DofusBinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public void Deserialize(DofusBinaryReader reader)
        {
            AlignmentSide = reader.ReadByte();
            AlignmentValue = reader.ReadByte();
            AlignmentGrade = reader.ReadByte();
            CharacterPower = reader.ReadDouble();
        }
    }
}
