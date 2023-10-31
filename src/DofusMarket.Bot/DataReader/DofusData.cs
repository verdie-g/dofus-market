using System.Diagnostics.CodeAnalysis;
using DofusMarket.Bot.Internationalization;

namespace DofusMarket.Bot.DataReader;

internal class DofusData
{
    private const string I18NFileFormat = "i18n_{0}.d2i";

    public static DofusData New(string dofusPath, string[]? dataTypeFilter = null)
    {
        string dofusDataCommonPath = Path.Combine(dofusPath, "data", "common");
        var data = Directory.GetFiles(dofusDataCommonPath)
            .Where(path =>
                path.EndsWith(".d2o", StringComparison.Ordinal)
                && (dataTypeFilter == null || Array.IndexOf(dataTypeFilter, Path.GetFileNameWithoutExtension(path)) != -1))
            .ToDictionary(path => Path.GetFileNameWithoutExtension(path)!, ReadD2O);

        string dofusDataI18N = Path.Combine(dofusPath, "data", "i18n");
        var textsByLanguages = DofusLanguages.All.ToDictionary(l => l, l =>
        {
            string filePath = Path.Combine(dofusDataI18N, string.Format(I18NFileFormat, l));
            using FileStream streamReader = new(filePath, FileMode.Open);
            D2IReader d2IReader = new(streamReader);
            return d2IReader.ReadAllText();
        });

        var undiacriticTextsByLanguages = DofusLanguages.All.ToDictionary(l => l, l =>
        {
            string filePath = Path.Combine(dofusDataI18N, string.Format(I18NFileFormat, l));
            using FileStream streamReader = new(filePath, FileMode.Open);
            D2IReader d2IReader = new(streamReader);
            return d2IReader.ReadUndiacriticalText();
        });

        return new DofusData(data, textsByLanguages, undiacriticTextsByLanguages);
    }

    private static Dictionary<int, Dictionary<string, object?>> ReadD2O(string path)
    {
        using FileStream streamReader = new(path, FileMode.Open);
        D2OReader d2OReader = new(streamReader);
        return d2OReader.ReadAllObjects();
    }

    private readonly Dictionary<string, Dictionary<int, Dictionary<string, object?>>> _data;
    private readonly Dictionary<string, Dictionary<int, string>> _textsByLanguages;
    private readonly Dictionary<string, Dictionary<int, string>> _undiacriticTextsByLanguages;

    private DofusData(
        Dictionary<string, Dictionary<int, Dictionary<string, object?>>> data,
        Dictionary<string, Dictionary<int, string>> textsByLanguages,
        Dictionary<string, Dictionary<int, string>> undiacriticTextsByLanguages)
    {
        _data = data;
        _textsByLanguages = textsByLanguages;
        _undiacriticTextsByLanguages = undiacriticTextsByLanguages;
    }

    public Dictionary<int, Dictionary<string, object?>> GetDataForType(string type)
    {
        return _data[type];
    }

    public Dictionary<string, object?> GetData(int id, string type)
    {
        return _data[type][id];
    }

    public string GetText(int textId, string lang)
    {
        return _textsByLanguages[lang][textId];
    }

    public string GetUndiacriticText(int textId, string lang)
    {
        return _undiacriticTextsByLanguages[lang][textId];
    }

    public bool TryGetText(int textId, string lang, [MaybeNullWhen(false)] out string text)
    {
        return _textsByLanguages[lang].TryGetValue(textId, out text);
    }

    public IEnumerable<int> GetTextIds(string text)
    {
        foreach (var languageTexts in _textsByLanguages.Values)
        {
            foreach (var languageText in languageTexts)
            {
                if (languageText.Value == text)
                {
                    yield return languageText.Key;
                }
            }
        }
    }
}