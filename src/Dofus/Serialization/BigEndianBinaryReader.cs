using System.Buffers.Binary;
using System.IO;
using System.Text;

namespace Dofus.Serialization
{
    internal class BigEndianBinaryReader : BinaryReader
    {
        public BigEndianBinaryReader(Stream input, Encoding encoding) : base(input, encoding)
        {
        }

        public override short ReadInt16()
            => BinaryPrimitives.ReadInt16BigEndian(ReadBytes(sizeof(short)));

        public override int ReadInt32() =>
            BinaryPrimitives.ReadInt32BigEndian(ReadBytes(sizeof(int)));

        public override long ReadInt64() =>
            BinaryPrimitives.ReadInt64BigEndian(ReadBytes(sizeof(long)));

        public override ushort ReadUInt16() =>
            BinaryPrimitives.ReadUInt16BigEndian(ReadBytes(sizeof(ushort)));

        public override uint ReadUInt32() =>
            BinaryPrimitives.ReadUInt32BigEndian(ReadBytes(sizeof(uint)));

        public override ulong ReadUInt64() =>
            BinaryPrimitives.ReadUInt64BigEndian(ReadBytes(sizeof(ulong)));

        public override float ReadSingle() =>
            BinaryPrimitives.ReadSingleBigEndian(ReadBytes(sizeof(float)));

        public override double ReadDouble() =>
            BinaryPrimitives.ReadDoubleBigEndian(ReadBytes(sizeof(double)));
    }
}
