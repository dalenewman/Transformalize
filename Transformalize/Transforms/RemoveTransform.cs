using System.Collections.Generic;
using System.Text;
using Transformalize.Model;

namespace Transformalize.Transforms {
    public class RemoveTransform : ITransform {
        private readonly int _startIndex;
        private readonly int _length;
        private readonly Dictionary<string, IField> _parameters;
        private readonly Dictionary<string, IField> _results;

        public RemoveTransform(int startIndex, int length) {
            _startIndex = startIndex;
            _length = length;
        }

        public RemoveTransform(int startIndex, int length, Dictionary<string, IField> parameters, Dictionary<string, IField> results) {
            _startIndex = startIndex;
            _length = length;
            _parameters = parameters;
            _results = results;
            HasParameters = parameters != null && parameters.Count > 0;
            HasResults = results != null && results.Count > 0;
        }

        public void Transform(StringBuilder sb) {
            sb.Remove(_startIndex, _length);
        }

        public object Transform(object value) {
            return value.ToString().Remove(_startIndex, _length);
        }

        public bool HasParameters { get; private set; }
        public bool HasResults { get; private set; }

        public void Dispose() { }
    }
}