using System.Collections.Generic;
using System.Text;
using Transformalize.Model;
using Transformalize.Rhino.Etl.Core;

namespace Transformalize.Transforms {
    public class TrimStartTransform : ITransform {
        private readonly string _trimChars;
        public Dictionary<string, Field> Parameters { get; private set; }
        public Dictionary<string, Field> Results { get; private set; }
        private readonly char[] _trimCharArray;

        public TrimStartTransform(string trimChars) {
            _trimChars = trimChars;
            _trimCharArray = trimChars.ToCharArray();
        }

        public TrimStartTransform(string trimChars, Dictionary<string, Field> parameters, Dictionary<string, Field> results) {
            _trimChars = trimChars;
            Parameters = parameters;
            Results = results;
            _trimCharArray = trimChars.ToCharArray();
            HasParameters = parameters != null && parameters.Count > 0;
            HasResults = results != null && results.Count > 0;
        }

        public void Transform(ref StringBuilder sb) {
            sb.TrimStart(_trimChars);
        }

        public void Transform(ref object value) {
            value = value.ToString().TrimStart(_trimCharArray);
        }
        public void Transform(ref Row row) {

        }

        public bool HasParameters { get; private set; }
        public bool HasResults { get; private set; }

        public void Dispose() { }
    }
}