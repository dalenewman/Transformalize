using System.Collections.Generic;
using System.Text;
using Transformalize.Model;

namespace Transformalize.Transforms {
    public class PadRightTransform : ITransform {
        private readonly int _totalWidth;
        private readonly char _paddingChar;
        private readonly Dictionary<string, IField> _parameters;
        private readonly Dictionary<string, IField> _results;

        public PadRightTransform(int totalWidth, char paddingChar) {
            _totalWidth = totalWidth;
            _paddingChar = paddingChar;
        }

        public PadRightTransform(int totalWidth, char paddingChar, Dictionary<string, IField> parameters, Dictionary<string, IField> results) {
            _totalWidth = totalWidth;
            _paddingChar = paddingChar;
            _parameters = parameters;
            _results = results;
            HasParameters = parameters != null && parameters.Count > 0;
            HasResults = results != null && results.Count > 0;
        }

        public void Transform(StringBuilder sb) {
            sb.PadRight(_totalWidth, _paddingChar);
        }

        public object Transform(object value) {
            return value.ToString().PadRight(_totalWidth, _paddingChar);
        }

        public bool HasParameters { get; private set; }
        public bool HasResults { get; private set; }

        public void Dispose() { }


    }
}