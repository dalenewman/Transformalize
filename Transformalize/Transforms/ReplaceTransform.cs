using System;
using System.Collections.Generic;
using System.Text;
using Transformalize.Model;

namespace Transformalize.Transforms {
    public class ReplaceTransform : ITransform {

        private readonly string _oldValue;
        private readonly string _newValue;
        private readonly Dictionary<string, IField> _parameters;
        private readonly Dictionary<string, IField> _results;

        public ReplaceTransform(string oldValue, string newValue) {
            _oldValue = oldValue;
            _newValue = newValue;
        }

        public ReplaceTransform(string oldValue, string newValue, Dictionary<string, IField> parameters, Dictionary<string, IField> results) {
            _oldValue = oldValue;
            _newValue = newValue;
            _parameters = parameters;
            _results = results;
            HasParameters = parameters != null && parameters.Count > 0;
            HasResults = results != null && results.Count > 0;
        }

        public void Transform(StringBuilder sb) {
            sb.Replace(_oldValue, _newValue);
        }

        public object Transform(object value) {
            return value.ToString().Replace(_oldValue, _newValue);
        }

        public bool HasParameters { get; private set; }
        public bool HasResults { get; private set; }

        public void Dispose() { }
    }
}