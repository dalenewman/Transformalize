using System.Collections.Generic;
using System.Text;
using Transformalize.Model;
using Transformalize.Rhino.Etl.Core;

namespace Transformalize.Transforms {
    public class LeftTransform : ITransform {
        private readonly int _length;
        public Dictionary<string, Field> Parameters { get; private set; }
        public Dictionary<string, Field> Results { get; private set; }
        private readonly bool _hasParameters;
        private readonly bool _hasResults;

        public LeftTransform(int length) {
            _length = length;
        }

        public LeftTransform(int length, Dictionary<string, Field> parameters, Dictionary<string, Field> results) {
            _length = length;
            Parameters = parameters;
            Results = results;
            _hasParameters = parameters != null && parameters.Count > 0;
            _hasResults = results != null && results.Count > 0;
        }

        public void Transform(ref StringBuilder sb) {
            sb.Left(_length);
        }

        public void Transform(ref object value) {
            value = value.ToString().Left(_length);
        }

        public void Transform(ref Row row) {

        }

        public bool HasParameters {
            get { return _hasParameters; }
        }

        public bool HasResults {
            get { return _hasResults; }
        }

        public void Dispose() { }
    }
}