using System;
using System.Buffers.Binary;
using System.IO;
using System.Text;

namespace Dofus.Serialization
{
    internal class BigEndianBinaryWriter : BinaryWriter
    {
        public BigEndianBinaryWriter(Stream input) : base(input)
        {
        }

        public BigEndianBinaryWriter(Stream input, Encoding encoding, bool leaveOpen) : base(input, encoding, leaveOpen)
        {
        }

        public override void Write(short value)
            => base.Write(BinaryPrimitives.ReverseEndianness(value));

        public override void Write(int value)
            => base.Write(BinaryPrimitives.ReverseEndianness(value));

        public override void Write(long value)
            => base.Write(BinaryPrimitives.ReverseEndianness(value));

        public override void Write(ushort value)
            => base.Write(BinaryPrimitives.ReverseEndianness(value));

        public override void Write(uint value)
            => base.Write(BinaryPrimitives.ReverseEndianness(value));

        public override void Write(ulong value)
            => base.Write(BinaryPrimitives.ReverseEndianness(value));

        public override void Write(float value)
            => throw new NotSupportedException();

        public override void Write(double value)
        {
            byte[] valueBytes = BitConverter.GetBytes(value);
            Array.Reverse(valueBytes);
            base.Write(valueBytes);
        }
    }
}
