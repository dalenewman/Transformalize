using System.Collections.Generic;
using System.Text;
using Transformalize.Model;

namespace Transformalize.Transforms {
    public class MapTransform : ITransform {
        private readonly Dictionary<string, IField> _parameters;
        private readonly Dictionary<string, IField> _results;
        private readonly IDictionary<string, object> _equals;
        private readonly IDictionary<string, object> _startsWith;
        private readonly IDictionary<string, object> _endsWith;

        public MapTransform(IList<IDictionary<string, object>> maps) {
            _equals = maps[0];
            _startsWith = maps[1];
            _endsWith = maps[2];
        }

        public MapTransform(IList<IDictionary<string, object>> maps, Dictionary<string, IField> parameters, Dictionary<string, IField> results) {
            _parameters = parameters;
            _results = results;
            _equals = maps[0];
            _startsWith = maps[1];
            _endsWith = maps[2];
            HasParameters = parameters != null && parameters.Count > 0;
            HasResults = results != null && results.Count > 0;
        }

        public void Transform(StringBuilder sb) {

            foreach (var key in _equals.Keys) {
                if (!sb.IsEqualTo(key)) continue;
                sb.Clear();
                sb.Append(_equals[key]);
                goto done;
            }

            foreach (var key in _startsWith.Keys) {
                if (!sb.StartsWith(key)) continue;
                sb.Clear();
                sb.Append(_startsWith[key]);
                goto done;
            }

            foreach (var key in _endsWith.Keys) {
                if (!sb.EndsWith(key)) continue;
                sb.Clear();
                sb.Append(_endsWith[key]);
                goto done;
            }

            foreach (var key in _equals.Keys) {
                if (!key.Equals("*")) continue;
                sb.Clear();
                sb.Append(_equals[key]);
                goto done;
            }

        done: ;
        }

        public object Transform(object value) {
            foreach (var key in _equals.Keys) {
                if (value.Equals(key))
                    return _equals[key];
            }

            foreach (var key in _startsWith.Keys) {
                if (value.ToString().StartsWith(key))
                    return _startsWith[key];
            }

            foreach (var key in _endsWith.Keys) {
                if (value.ToString().EndsWith(key))
                    return _endsWith[key];
            }

            foreach (var key in _equals.Keys) {
                if (key.Equals("*"))
                    return _equals[key];
            }

            return value;

        }

        public bool HasParameters { get; private set; }
        public bool HasResults { get; private set; }

        public void Dispose() { }
    }
}