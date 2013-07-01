using System.Collections.Generic;
using System.Text;
using Transformalize.Model;
using Transformalize.Rhino.Etl.Core;

namespace Transformalize.Transforms {
    public class TrimTransform : ITransform {
        private readonly string _trimChars;
        private readonly char[] _trimCharArray;
        public Dictionary<string, Field> Parameters { get; private set; }
        public Dictionary<string, Field> Results { get; private set; }
        private readonly bool _hasParameters;
        private readonly bool _hasResults;

        public bool HasParameters {
            get { return _hasParameters; }
        }

        public bool HasResults {
            get { return _hasResults; }
        }

        public TrimTransform(string trimChars) {
            _trimChars = trimChars;
            _trimCharArray = trimChars.ToCharArray();
        }

        public TrimTransform(string trimChars, Dictionary<string, Field> parameters, Dictionary<string, Field> results) {
            _trimCharArray = trimChars.ToCharArray();
            _trimChars = trimChars;
            Parameters = parameters;
            Results = results;
            _hasParameters = parameters != null && parameters.Count > 0;
            _hasResults = results != null && results.Count > 0;
        }

        public void Transform(ref StringBuilder sb) {
            sb.Trim(_trimChars);
        }

        public void Transform(ref object value) {
            value = value.ToString().Trim(_trimCharArray);
        }

        public void Transform(ref Row row) {

        }

        public void Dispose() { }
    }
}