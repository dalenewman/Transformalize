using System.Collections.Generic;
using System.Text;
using Transformalize.Model;
using Transformalize.Rhino.Etl.Core;

namespace Transformalize.Transforms {
    public class InsertTransform : ITransform {

        private readonly int _index;
        private readonly string _value;
        private readonly Dictionary<string, Field> _parameters;
        private readonly Dictionary<string, Field> _results;

        public bool HasParameters { get; private set; }
        public bool HasResults { get; private set; }

        public InsertTransform(int index, string value) {
            _index = index;
            _value = value;
            HasParameters = false;
            HasResults = false;
        }

        public InsertTransform(int index, string value, Dictionary<string, Field> parameters, Dictionary<string, Field> results) {
            _index = index;
            _value = value;
            _parameters = parameters;
            _results = results;
            HasParameters = parameters != null && parameters.Count > 0;
            HasResults = results != null && results.Count > 0;
        }

        public void Transform(ref StringBuilder sb) {
            if (_index > sb.Length) return;
            sb.Insert(_index, _value);
        }

        public void Transform(ref object value) {
            var str = value.ToString();
            if (_index > str.Length) return;
            value = str.Insert(_index, _value);
        }

        public void Transform(ref Row row) {

        }

        public void Dispose() { }
    }
}