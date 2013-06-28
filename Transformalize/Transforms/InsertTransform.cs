using System.Collections.Generic;
using System.Text;
using Transformalize.Model;

namespace Transformalize.Transforms {
    public class InsertTransform : ITransform {

        private readonly int _index;
        private readonly string _value;
        private readonly Dictionary<string, IField> _parameters;
        private readonly Dictionary<string, IField> _results;

        public bool HasParameters { get; private set; }
        public bool HasResults { get; private set; }

        public InsertTransform(int index, string value) {
            _index = index;
            _value = value;
            HasParameters = false;
            HasResults = false;
        }

        public InsertTransform(int index, string value, Dictionary<string, IField> parameters, Dictionary<string, IField> results) {
            _index = index;
            _value = value;
            _parameters = parameters;
            _results = results;
            HasParameters = parameters != null && parameters.Count > 0;
            HasResults = results != null && results.Count > 0;
        }

        public void Transform(StringBuilder sb) {
            sb.Insert(_index, _value);
        }

        public object Transform(object value) {
            return value.ToString().Insert(_index, _value);
        }

        public void Dispose() { }
    }
}