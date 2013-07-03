using System.Collections.Generic;
using System.Text;
using Transformalize.Model;
using Transformalize.Rhino.Etl.Core;

namespace Transformalize.Transforms {
    public class MapTransform : ITransform {
        public Dictionary<string, Field> Parameters { get; private set; }
        public Dictionary<string, Field> Results { get; private set; }
        private readonly IDictionary<string, object> _equals;
        private readonly IDictionary<string, object> _startsWith;
        private readonly IDictionary<string, object> _endsWith;

        public MapTransform(IList<IDictionary<string, object>> maps) {
            _equals = maps[0];
            _startsWith = maps[1];
            _endsWith = maps[2];
        }

        public MapTransform(IList<IDictionary<string, object>> maps, Dictionary<string, Field> parameters, Dictionary<string, Field> results) {
            Parameters = parameters;
            Results = results;
            _equals = maps[0];
            _startsWith = maps[1];
            _endsWith = maps[2];
            HasParameters = parameters != null && parameters.Count > 0;
            HasResults = results != null && results.Count > 0;
        }

        public void Transform(ref StringBuilder sb) {

            foreach (var pair in _equals) {
                if (!sb.IsEqualTo(pair.Key)) continue;
                sb.Clear();
                sb.Append(_equals[pair.Key]);
                return;
            }

            foreach (var pair in _startsWith) {
                if (!sb.StartsWith(pair.Key)) continue;
                sb.Clear();
                sb.Append(_startsWith[pair.Key]);
                return;
            }

            foreach (var pair in _endsWith) {
                if (!sb.EndsWith(pair.Key)) continue;
                sb.Clear();
                sb.Append(_endsWith[pair.Key]);
                return;
            }

            foreach (var pair in _equals) {
                if (!pair.Key.Equals("*")) continue;
                sb.Clear();
                sb.Append(_equals[pair.Key]);
                return;
            }

        }

        public void Transform(ref object value) {
            foreach (var key in _equals.Keys) {
                if (!value.Equals(key)) continue;
                value = _equals[key];
                return;
            }

            foreach (var key in _startsWith.Keys) {
                if (!value.ToString().StartsWith(key)) continue;
                value = _startsWith[key];
                return;
            }

            foreach (var key in _endsWith.Keys) {
                if (!value.ToString().EndsWith(key)) continue;
                value = _endsWith[key];
                return;
            }

            foreach (var key in _equals.Keys) {
                if (!key.Equals("*")) continue;
                value = _equals[key];
                return;
            }

        }

        public void Transform(ref Row row)
        {
            
        }

        public bool HasParameters { get; private set; }
        public bool HasResults { get; private set; }

        public void Dispose() { }
    }
}