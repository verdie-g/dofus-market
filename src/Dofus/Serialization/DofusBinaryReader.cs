using System;
using System.IO;
using System.Text;
using Dofus.Messages;

namespace Dofus.Serialization
{
    public class DofusBinaryReader : IDisposable
    {
        private readonly BigEndianBinaryReader _reader;

        public DofusBinaryReader(Stream stream) => _reader = new BigEndianBinaryReader(stream, Encoding.UTF8);
        public Stream BaseStream => _reader.BaseStream;
        public byte[] ReadBytes(int count) => _reader.ReadBytes(count);
        public bool ReadBoolean() => _reader.ReadBoolean();
        public byte ReadByte() => _reader.ReadByte();
        public short ReadInt16() => _reader.ReadInt16();
        public ushort ReadUInt16() => _reader.ReadUInt16();
        public uint ReadUInt32() => _reader.ReadUInt32();
        public int ReadInt32() => _reader.ReadInt32();
        public long ReadInt64() => (long)_reader.ReadDouble(); // AS3 doesn't support Int64.
        public float ReadSingle() => _reader.ReadSingle();
        public double ReadDouble() => _reader.ReadDouble();
        public int Read7BitEncodedInt() => _reader.Read7BitEncodedInt();
        public long Read7BitEncodedInt64() => _reader.Read7BitEncodedInt64();

        public string ReadString()
        {
            ushort length = _reader.ReadUInt16();
            byte[] strBytes = _reader.ReadBytes(length);
            return Encoding.UTF8.GetString(strBytes);
        }

        public TObject ReadObject<TObject>(bool polymorphic = false)
            where TObject : INetworkMessage
        {
            Type objectType;
            if (polymorphic)
            {
                ushort objectTypeId = ReadUInt16();
                objectType = NetworkMessageRegistry.GetTypeFromId(objectTypeId);
            }
            else
            {
                objectType = typeof(TObject);
            }

            INetworkMessage obj = (INetworkMessage)Activator.CreateInstance(objectType)!;
            obj.Deserialize(this);
            return (TObject)obj;
        }

        public TObject[] ReadObjectCollection<TObject>(bool polymorphic = false)
            where TObject : INetworkMessage
        {
            return ReadCollection(r => r.ReadObject<TObject>(polymorphic));
        }

        public T[] ReadCollection<T>(Func<DofusBinaryReader, T> deserializeElement)
        {
            ushort length = ReadUInt16();
            T[] elements = new T[length];
            for (ushort i = 0; i < length; i += 1)
            {
                elements[i] = deserializeElement(this);
            }

            return elements;
        }

        public void Dispose() => _reader.Dispose();
    }
}
