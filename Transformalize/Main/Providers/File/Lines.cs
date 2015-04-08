using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Transformalize.Logging;

namespace Transformalize.Main.Providers.File {

    public class Lines {

        private readonly FileSystemInfo _fileInfo;
        private readonly FileInspectionRequest _request;
        private readonly List<Line> _storage = new List<Line>();
        private char _bestDelimiter;
        private bool _isCsv;

        public Lines(FileSystemInfo fileInfo, FileInspectionRequest request) {
            _fileInfo = fileInfo;
            _isCsv = fileInfo.Extension.Equals(".csv", StringComparison.OrdinalIgnoreCase);
            _request = request;
            _storage.AddRange(new LineLoader(fileInfo, request).Load());
        }

        public char FindDelimiter() {

            if (_bestDelimiter != default(char))
                return _bestDelimiter;

            if (_storage.Count == 0) {
                TflLogger.Warn(string.Empty, string.Empty, "Can't find any lines or a delimiter for {0}. Defaulting to single column.", _fileInfo.Name);
                return default(char);
            }

            var isSample = _storage.Count == _request.LineLimit;
            var count = Convert.ToDouble(_storage.Count);

            foreach (var pair in _request.Delimiters) {
                var delimiter = pair.Key;
                var values = _storage.Select(l => l.Values[delimiter].Length - 1).ToArray();
                var average = values.Average();
                var min = values.Min();
                if (min == 0 || !(average > 0))
                    continue;

                var variance = values.Sum(l => Math.Pow((l - average), 2)) / (count-(isSample ? 1 : 0));

                pair.Value.AveragePerLine = average;
                pair.Value.StandardDeviation = _storage.Count == 1 ? 0 : Math.Sqrt(variance);
            }

            var winner = _request.Delimiters
                .Select(p => p.Value)
                .Where(d => d.AveragePerLine > 0)
                .OrderBy(d => d.CoefficientOfVariance())
                .ThenByDescending(d=> d.AveragePerLine)
                .FirstOrDefault();

            if (winner != null) {
                _bestDelimiter = winner.Character;
                TflLogger.Info(string.Empty, string.Empty, "Delimiter is '{0}'", _bestDelimiter);
                return _bestDelimiter;
            }

            TflLogger.Warn(string.Empty, string.Empty, "Can't find a delimiter for {0}.  Defaulting to single column.", _fileInfo.Name);
            return default(char);
        }

        public char FindDelimiterOld() {

            if (_bestDelimiter != default(char))
                return _bestDelimiter;

            var max = 0;
            var candidates = new Dictionary<char, int>();

            foreach (var delimiter in _request.Delimiters.Keys) {
                foreach (var line in _storage) {
                    var count = line.Values[delimiter].Length - 1;
                    if (count > 0 && _storage.All(l => l.Values[delimiter].Length - 1 == count)) {
                        candidates[delimiter] = count;
                        if (count > max) {
                            max = count;
                        }
                    }
                }
            }

            if (!candidates.Any()) {
                TflLogger.Warn(string.Empty, string.Empty, "Can't find a delimiter for {0}.  Defaulting to single column.", _fileInfo.Name);
                return default(char);
            }

            _bestDelimiter = candidates.First(kv => kv.Value.Equals(max)).Key;
            TflLogger.Info(string.Empty, string.Empty, "Delimiter is '{0}'", _bestDelimiter);
            return _bestDelimiter;
        }

        public Fields InitialFieldTypes() {

            var fields = new Fields();
            var delimiter = FindDelimiter();

            if (_storage.Count == 0) {
                TflLogger.Warn("JunkDrawer", _fileInfo.Name, "No lines in file.");
                return fields;
            }

            var firstLine = _storage[0];

            if (delimiter == default(char)) {
                var field = new Field("string", _request.DefaultLength, FieldType.NonKey, true, string.Empty) {
                    Name = firstLine.Content
                };
                fields.Add(field);
                return fields;
            }

            var names = firstLine.Values[delimiter].Select(n=>n.Trim(firstLine.Quote)).ToArray();

            for (var i = 0; i < names.Length; i++) {
                var name = names[i];
                var field = new Field(_request.DefaultType, _request.DefaultLength, FieldType.NonKey, true, string.Empty) {
                    Name = name,
                    QuotedWith = firstLine.Quote
                };
                fields.Add(field);
            }

            return fields;
        }

    }
}