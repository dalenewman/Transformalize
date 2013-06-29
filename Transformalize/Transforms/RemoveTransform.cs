using System.Collections.Generic;
using System.Text;
using Transformalize.Model;
using Transformalize.Rhino.Etl.Core;

namespace Transformalize.Transforms {
    public class RemoveTransform : ITransform {
        private readonly int _startIndex;
        private readonly int _length;
        private readonly Dictionary<string, Field> _parameters;
        private readonly Dictionary<string, Field> _results;

        public RemoveTransform(int startIndex, int length) {
            _startIndex = startIndex;
            _length = length;
        }

        public RemoveTransform(int startIndex, int length, Dictionary<string, Field> parameters, Dictionary<string, Field> results) {
            _startIndex = startIndex;
            _length = length;
            _parameters = parameters;
            _results = results;
            HasParameters = parameters != null && parameters.Count > 0;
            HasResults = results != null && results.Count > 0;
        }

        public void Transform(ref StringBuilder sb) {
            if (_startIndex > sb.Length) return;
            sb.Remove(_startIndex, _length);
        }

        public void Transform(ref object value) {
            var str = value.ToString();
            if (_startIndex > str.Length) return;
            value = str.Remove(_startIndex, _length);
        }

        public void Transform(ref Row row) {

        }

        public bool HasParameters { get; private set; }
        public bool HasResults { get; private set; }

        public void Dispose() { }
    }
}