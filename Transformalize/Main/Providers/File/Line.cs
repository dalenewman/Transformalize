using System.Collections.Generic;
using Transformalize.Extensions;

namespace Transformalize.Main.Providers.File {

    public class Line {

        private readonly char _quote = default(char);
        private readonly string _content = string.Empty;
        private readonly Dictionary<char, string[]> _values = new Dictionary<char, string[]>();

        public string Content {
            get { return _content; }
        }

        public Dictionary<char, string[]> Values {
            get { return _values; }
        }

        public char Quote { get { return _quote; } }

        public Line(string content, FileInspectionRequest request) {
            _content = content;
            foreach (var pair in request.Delimiters) {
                _values[pair.Key] = content.Split(pair.Key);
            }
        }

        public Line(string content, char quote, FileInspectionRequest request) {
            _content = content;
            _quote = quote;
            foreach (var pair in request.Delimiters) {
                _values[pair.Key] = content.DelimiterSplit(pair.Key, quote);
            }
        }

    }
}