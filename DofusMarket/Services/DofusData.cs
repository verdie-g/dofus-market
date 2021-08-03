using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Dofus.DataReader;

namespace DofusMarket.Services
{
    internal class DofusData
    {
        public static DofusData New(string folder)
        {
            var data = Directory.GetFiles(folder)
                .Where(path => path.EndsWith(".d2o", StringComparison.Ordinal))
                .ToDictionary(path => Path.GetFileNameWithoutExtension(path)!, ReadD2O);
            return new DofusData(data);
        }

        private static Dictionary<int, Dictionary<string, object?>> ReadD2O(string path)
        {
            using FileStream streamReader = new(path, FileMode.Open);
            D2OReader d2OReader = new(streamReader);
            return d2OReader.ReadAllObjects();
        }

        private readonly Dictionary<string, Dictionary<int, Dictionary<string, object?>>> _data;

        private DofusData(Dictionary<string, Dictionary<int, Dictionary<string, object?>>> data)
        {
            _data = data;
        }

        public Dictionary<string, object?> GetData(int id, string type)
        {
            return _data[type][id];
        }
    }
}
