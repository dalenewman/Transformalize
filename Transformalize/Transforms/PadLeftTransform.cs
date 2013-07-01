using System.Collections.Generic;
using System.Text;
using Transformalize.Model;
using Transformalize.Rhino.Etl.Core;

namespace Transformalize.Transforms {
    public class PadLeftTransform : ITransform {
        private readonly int _totalWidth;
        private readonly char _paddingChar;
        public Dictionary<string, Field> Parameters { get; private set; }
        public Dictionary<string, Field> Results { get; private set; }

        public PadLeftTransform(int totalWidth, char paddingChar) {
            _totalWidth = totalWidth;
            _paddingChar = paddingChar;
        }

        public PadLeftTransform(int totalWidth, char paddingChar, Dictionary<string, Field> parameters, Dictionary<string, Field> results) {
            _totalWidth = totalWidth;
            _paddingChar = paddingChar;
            Parameters = parameters;
            Results = results;
            HasParameters = parameters != null && parameters.Count > 0;
            HasResults = results != null && results.Count > 0;
        }

        public void Transform(ref StringBuilder sb) {
            sb.PadLeft(_totalWidth, _paddingChar);
        }

        public void Transform(ref object value) {
            value = value.ToString().PadLeft(_totalWidth, _paddingChar);
        }

        public void Transform(ref Row row)
        {
            
        }

        public bool HasParameters { get; private set; }
        public bool HasResults { get; private set; }

        public void Dispose() { }
    }
}