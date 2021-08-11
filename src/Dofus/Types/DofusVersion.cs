using Dofus.Messages;
using Dofus.Serialization;

namespace Dofus.Types
{
    public class DofusVersion : INetworkMessage
    {
        internal static ushort MessageId => 4016;

        public byte Major { get; set; }
        public byte Minor { get; set; }
        public byte Code { get; set; }
        public int Build { get; set; }
        public BuildType BuildType { get; set; }

        public void Serialize(DofusBinaryWriter writer)
        {
            writer.Write(Major);
            writer.Write(Minor);
            writer.Write(Code);
            writer.Write(Build);
            writer.Write((byte)BuildType);
        }

        public void Deserialize(DofusBinaryReader reader)
        {
            Major = reader.ReadByte();
            Minor = reader.ReadByte();
            Code = reader.ReadByte();
            Build = reader.ReadInt32();
            BuildType = (BuildType)reader.ReadByte();
        }

        public override string ToString()
        {
            return $"{Major}.{Minor}.{Code}.{Build}";
        }
    }
}
