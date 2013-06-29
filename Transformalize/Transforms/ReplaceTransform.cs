using System;
using System.Collections.Generic;
using System.Text;
using Transformalize.Model;
using Transformalize.Rhino.Etl.Core;

namespace Transformalize.Transforms {
    public class ReplaceTransform : ITransform {

        private readonly string _oldValue;
        private readonly string _newValue;
        private readonly Dictionary<string, Field> _parameters;
        private readonly Dictionary<string, Field> _results;

        public ReplaceTransform(string oldValue, string newValue) {
            _oldValue = oldValue;
            _newValue = newValue;
        }

        public ReplaceTransform(string oldValue, string newValue, Dictionary<string, Field> parameters, Dictionary<string, Field> results) {
            _oldValue = oldValue;
            _newValue = newValue;
            _parameters = parameters;
            _results = results;
            HasParameters = parameters != null && parameters.Count > 0;
            HasResults = results != null && results.Count > 0;
        }

        public void Transform(ref StringBuilder sb) {
            sb.Replace(_oldValue, _newValue);
        }

        public void Transform(ref object value) {
            value = value.ToString().Replace(_oldValue, _newValue);
        }

        public void Transform(ref Row row) {

        }

        public bool HasParameters { get; private set; }
        public bool HasResults { get; private set; }

        public void Dispose() { }
    }
}