using System.Collections.Generic;
using System.IO;
using System.Linq;
using Dofus.DataReader;
using Dofus.Internationalization;

namespace DofusMarket.Services
{
    internal class DofusTexts
    {
        private const string FileFormat = "i18n_{0}.d2i";

        public static DofusTexts New(string folder)
        {
            var textsByLanguages = DofusLanguages.All.ToDictionary(l => l, l =>
            {
                string filePath = Path.Combine(folder, string.Format(FileFormat, l));
                using FileStream streamReader = new(filePath, FileMode.Open);
                D2IReader d2IReader = new(streamReader);
                return d2IReader.ReadAllText();
            });

            return new DofusTexts(textsByLanguages);
        }

        private readonly Dictionary<string, Dictionary<int, string>> _textsByLanguages;

        private DofusTexts(Dictionary<string, Dictionary<int, string>> textsByLanguages)
        {
            _textsByLanguages = textsByLanguages;
        }

        public string GetText(int textId, string lang)
        {
            return _textsByLanguages[lang][textId];
        }
    }
}
