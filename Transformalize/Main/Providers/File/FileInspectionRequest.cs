using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Transformalize.Configuration;

namespace Transformalize.Main.Providers.File {

    public class FileInspectionRequest {

        private static readonly char UnitSeparator = Convert.ToChar(31);
        private List<string> _dataTypes = new List<string> { "boolean", "int", "long", "decimal", "datetime" };
        private bool _ignoreEmpty = true;
        private string _defaultLength = "1024";
        private string _defaultType = "string";
        private int _lineLimit = 100;

        private Dictionary<char, TflDelimiter> _delimiters = new Dictionary<char, TflDelimiter> {
            { ',', new TflDelimiter { Name = "comma", Character = ','} },
            { '\t', new TflDelimiter { Name = "tab", Character = '\t'}},
            { '|', new TflDelimiter { Name = "pipe", Character = '|'}},
            { ';', new TflDelimiter { Name = "semicolon", Character = ';'} },
            { UnitSeparator, new TflDelimiter { Name = "unit", Character = UnitSeparator}}
        };

        public FileInfo FileInfo { get; set; }
        public string ProcessName { get; set; }
        public string EntityName { get; set; }

        public int LineLimit {
            get { return _lineLimit; }
            set { _lineLimit = value; }
        }

        public decimal Sample { get; set; }
        public int MaxLength { get; set; }
        public int MinLength { get; set; }

        public bool IgnoreEmpty {
            get { return _ignoreEmpty; }
            set { _ignoreEmpty = value; }
        }

        public List<string> DataTypes {
            get { return _dataTypes; }
            set { _dataTypes = value; }
        }

        public Dictionary<char, TflDelimiter> Delimiters {
            get { return _delimiters; }
            set {
                _delimiters = value;
                if (_delimiters.ContainsKey(UnitSeparator)) {
                    _delimiters[UnitSeparator] = new TflDelimiter() { Name = "unit", Character = UnitSeparator };
                }
            }
        }

        public string DefaultLength {
            get { return _defaultLength; }
            set { _defaultLength = value; }
        }

        public string DefaultType {
            get { return _defaultType; }
            set { _defaultType = value; }
        }

        public FileInspectionRequest(string fileName) {

            FileInfo = new FileInfo(fileName);

            //defaults to some safe names
            ProcessName = Common.CleanIdentifier(Path.GetFileNameWithoutExtension(FileInfo.Name));
            EntityName = ProcessName == string.Empty ? "TflAuto" + ProcessName.GetHashCode().ToString(CultureInfo.InvariantCulture).Replace("-", "0").PadRight(13, '0') : ProcessName;

        }

    }
}