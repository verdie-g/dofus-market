using System;
using System.IO;
using System.Text;

namespace Dofus.Serialization
{
    public class DofusBinaryWriter : IDisposable
    {
        private readonly BigEndianBinaryWriter _writer;

        public DofusBinaryWriter(Stream stream) => _writer = new BigEndianBinaryWriter(stream);
        public DofusBinaryWriter(Stream stream, bool leaveOpen) => _writer = new BigEndianBinaryWriter(stream, Encoding.UTF8, leaveOpen);
        public Stream BaseStream => _writer.BaseStream;
        public void Write(ReadOnlySpan<byte> value) => _writer.Write(value);
        public void Write(bool value) => _writer.Write(value);
        public void Write(byte value) => _writer.Write(value);
        public void Write(ushort value) => _writer.Write(value);
        public void Write(short value) => _writer.Write(value);
        public void Write(uint value) => _writer.Write(value);
        public void Write(int value) => _writer.Write(value);
        public void Write(long value) => _writer.Write((double)value); // AS3 doesn't support Int64.
        public void Write(double value) => _writer.Write(value);

        public void Write(string value)
        {
            _writer.Write((ushort)value.Length);
            byte[] strBytes = Encoding.UTF8.GetBytes(value);
            _writer.Write(strBytes);
        }

        public void Write7BitEncodedInt(int value) => _writer.Write7BitEncodedInt(value);
        public void Write7BitEncodedInt64(long value) => _writer.Write7BitEncodedInt64(value);
        public void Dispose() => _writer.Dispose();
    }
}
