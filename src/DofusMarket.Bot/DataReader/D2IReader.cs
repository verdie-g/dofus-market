using DofusMarket.Bot.Serialization;

namespace DofusMarket.Bot.DataReader;

public class D2IReader
{
    private readonly DofusBinaryReader _reader;

    private bool _checkPreamble = true;
    private Dictionary<int, int>? _indexes;
    private Dictionary<int, int>? _unDiacriticalIndex;
    private Dictionary<string, int>? _textIndexes;
    private Dictionary<int, int>? _textSortIndexes;

    public D2IReader(Stream stream)
    {
        _reader = new DofusBinaryReader(stream);
    }

    public string ReadText(int textId)
    {
        ReadPreamble();

        int pointer = _indexes![textId];
        _reader.BaseStream.Seek(pointer, SeekOrigin.Begin);
        return _reader.ReadString();
    }

    public Dictionary<int, string> ReadAllText()
    {
        ReadPreamble();

        return _indexes!.ToDictionary(i => i.Key, i => ReadText(i.Key));
    }

    public Dictionary<int, string> ReadUndiacriticalText()
    {
        ReadPreamble();

        return _indexes!.ToDictionary(i => i.Key, i => ReadUndiacriticalText(i.Key));
    }

    public string ReadUndiacriticalText(int textId)
    {
        ReadPreamble();

        int pointer = _unDiacriticalIndex![textId];
        _reader.BaseStream.Seek(pointer, SeekOrigin.Begin);
        return _reader.ReadString();
    }

    private void ReadPreamble()
    {
        if (_checkPreamble)
        {
            _checkPreamble = false;
            (_indexes, _unDiacriticalIndex) = ReadIndexes();
            _textIndexes = ReadTextIndexes();
            _textSortIndexes = ReadTextSortIndexes();
        }
    }

    private (Dictionary<int, int> indexes, Dictionary<int, int> unDiacriticalIndex) ReadIndexes()
    {
        Dictionary<int, int> indexes = new();
        Dictionary<int, int> unDiacriticalIndex = new();

        int indexesPointer = _reader.ReadInt32();
        _reader.BaseStream.Seek(indexesPointer, SeekOrigin.Begin);

        int indexesLength = _reader.ReadInt32();
        for (int i = 0; i < indexesLength; i += 9)
        {
            int key = _reader.ReadInt32();
            bool diacriticalText = _reader.ReadBoolean();
            int pointer = _reader.ReadInt32();
            indexes[key] = pointer;

            if (diacriticalText)
            {
                i += 4;
                unDiacriticalIndex[key] = _reader.ReadInt32();
            }
            else
            {
                unDiacriticalIndex[key] = pointer;
            }
        }

        return (indexes, unDiacriticalIndex);
    }

    private Dictionary<string, int> ReadTextIndexes()
    {
        Dictionary<string, int> textIndexes = new(StringComparer.Ordinal);
        int indexesLength = _reader.ReadInt32();
        while (indexesLength > 0)
        {
            long position = _reader.BaseStream.Position;
            string textKey = _reader.ReadString();
            int pointer = _reader.ReadInt32();
            textIndexes[textKey] = pointer;
            indexesLength -= (int)(_reader.BaseStream.Position - position);
        }

        return textIndexes;
    }

    private Dictionary<int, int> ReadTextSortIndexes()
    {
        Dictionary<int, int> textSortIndexes = new();
        int j = 0;
        int indexesLength = _reader.ReadInt32();
        while (indexesLength > 0)
        {
            long position = _reader.BaseStream.Position;
            int key = _reader.ReadInt32();
            textSortIndexes[key] = ++j;
            indexesLength -= (int)(_reader.BaseStream.Position - position);
        }

        return textSortIndexes;
    }
}