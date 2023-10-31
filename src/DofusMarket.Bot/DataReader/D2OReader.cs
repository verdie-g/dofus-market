using System.Text;
using DofusMarket.Bot.Serialization;

namespace DofusMarket.Bot.DataReader;

public class D2OReader
{
    private static readonly byte[] MagicBytes = Encoding.ASCII.GetBytes("D2O");

    private readonly DofusBinaryReader _reader;

    private bool _checkPreamble = true;
    private Dictionary<int, int>? _indexes;
    private Dictionary<int, GameDataClassDefinition>? _classes;

    public D2OReader(Stream stream)
    {
        _reader = new DofusBinaryReader(stream);
    }

    public Dictionary<string, object?> ReadObject(int objectId)
    {
        ReadPreamble();

        _reader.BaseStream.Position = _indexes![objectId];
        int classId = _reader.ReadInt32();
        GameDataClassDefinition classDef = _classes![classId];
        return ReadClassInstance(classDef);
    }

    public Dictionary<int, Dictionary<string, object?>> ReadAllObjects()
    {
        ReadPreamble();
        Dictionary<int, Dictionary<string, object?>> objects = new(capacity: _indexes!.Count);
        foreach (var index in _indexes)
        {
            objects[index.Key] = ReadObject(index.Key);
        }

        return objects;
    }

    private void ReadPreamble()
    {
        if (_checkPreamble)
        {
            byte[] magicBytes = _reader.ReadBytes(MagicBytes.Length);
            if (!magicBytes.SequenceEqual(MagicBytes))
            {
                throw new InvalidOperationException("Not a D2O file");
            }

            _checkPreamble = false;
            _indexes = ReadIndexes();
            _classes = ReadClasses();
        }
    }

    private Dictionary<int, int> ReadIndexes()
    {
        Dictionary<int, int> indexes = new();

        int indexesPointer = _reader.ReadInt32();
        _reader.BaseStream.Seek(indexesPointer, SeekOrigin.Begin);

        int indexesLength = _reader.ReadInt32();
        for (int i = 0; i < indexesLength; i += 8)
        {
            int key = _reader.ReadInt32();
            int pointer = _reader.ReadInt32();
            indexes[key] = pointer;
        }

        return indexes;
    }

    private Dictionary<int, GameDataClassDefinition> ReadClasses()
    {
        Dictionary<int, GameDataClassDefinition> classes = new();

        int classesCount = _reader.ReadInt32();
        for (int i = 0; i < classesCount; i += 1)
        {
            int classIdentifier = _reader.ReadInt32();
            var classDef = ReadClass();
            classes[classIdentifier] = classDef;
        }

        return classes;
    }

    private GameDataClassDefinition ReadClass()
    {
        string className = _reader.ReadString();
        string packageName = _reader.ReadString();

        int fieldsCount = _reader.ReadInt32();
        var fields = new GameDataField[fieldsCount];
        for (int i = 0; i < fieldsCount; i += 1)
        {
            fields[i] = ReadClassField();
        }

        return new GameDataClassDefinition(packageName, className, fields);
    }

    private GameDataField ReadClassField()
    {
        string fieldName = _reader.ReadString();
        var fieldType = (GameDataType)_reader.ReadInt32();
        GameDataField? innerField = fieldType == GameDataType.Vector ? ReadClassField() : null;
        return new GameDataField(fieldName, fieldType, innerField);
    }

    private Dictionary<string, object?> ReadClassInstance(GameDataClassDefinition classDef)
    {
        Dictionary<string, object?> instance = new();

        foreach (GameDataField field in classDef.Fields)
        {
            instance[field.Name] = ReadFieldValue(field);
        }

        return instance;
    }

    private object? ReadFieldValue(GameDataField field)
    {
        return field.Type switch
        {
            GameDataType.Int => _reader.ReadInt32(),
            GameDataType.Boolean => _reader.ReadBoolean(),
            GameDataType.String => ReadFieldValueString(),
            GameDataType.Number => _reader.ReadDouble(),
            GameDataType.I18N => _reader.ReadInt32(),
            GameDataType.UInt => _reader.ReadUInt32(),
            GameDataType.Vector => ReadFieldValueVector(field),
            _ => ReadFieldValueObject(),
        };
    }

    private string? ReadFieldValueString()
    {
        string s = _reader.ReadString();
        return s == "null" ? null : s;
    }

    private object?[] ReadFieldValueVector(GameDataField field)
    {
        int length = _reader.ReadInt32();
        var vector = new object?[length];
        for (int i = 0; i < length; i += 1)
        {
            vector[i] = ReadFieldValue(field.InnerField!);
        }

        return vector;
    }

    private Dictionary<string, object?>? ReadFieldValueObject()
    {
        int classId = _reader.ReadInt32();
        if (classId == -1431655766)
        {
            return null;
        }

        GameDataClassDefinition classDef = _classes![classId];
        return ReadClassInstance(classDef);
    }

    private enum GameDataType
    {
        Int = -1,
        Boolean = -2,
        String = -3,
        Number = -4,
        I18N = -5,
        UInt = -6,
        Vector = -99,
    }

    private record GameDataField(string Name, GameDataType Type, GameDataField? InnerField);

    private record GameDataClassDefinition(string PackageName, string ClassName, GameDataField[] Fields);
}