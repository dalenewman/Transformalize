using System.Collections.Generic;
using System.Text;
using Transformalize.Model;

namespace Transformalize.Transforms {
    public class SubstringTransform : ITransform {
        private readonly int _startIndex;
        private readonly int _length;
        private readonly Dictionary<string, IField> _parameters;
        private readonly Dictionary<string, IField> _results;

        public SubstringTransform(int startIndex, int length) {
            _startIndex = startIndex;
            _length = length;
        }

        public SubstringTransform(int startIndex, int length, Dictionary<string, IField> parameters, Dictionary<string, IField> results) {
            _startIndex = startIndex;
            _length = length;
            _parameters = parameters;
            _results = results;
            HasParameters = parameters != null && parameters.Count > 0;
            HasResults = results != null && results.Count > 0;
        }

        public void Transform(StringBuilder sb) {
            sb.Substring(_startIndex, _length);
        }

        public object Transform(object value) {
            return value.ToString().Substring(_startIndex, _length);
        }

        public bool HasParameters { get; private set; }
        public bool HasResults { get; private set; }

        public void Dispose() { }
    }
}