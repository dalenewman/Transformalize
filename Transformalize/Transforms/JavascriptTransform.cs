using System.Collections.Generic;
using System.Text;
using Noesis.Javascript;
using Transformalize.Model;

namespace Transformalize.Transforms {
    public class JavascriptTransform : ITransform {
        private readonly JavascriptContext _context = new JavascriptContext();
        private readonly string _script;
        private readonly Dictionary<string, IField> _parameters;
        private readonly Dictionary<string, IField> _results;

        public JavascriptTransform(string script) {
            _script = script;
        }

        public JavascriptTransform(string script, Dictionary<string, IField> parameters, Dictionary<string, IField> results) {
            _script = script;
            _parameters = parameters;
            _results = results;
            HasParameters = parameters != null && parameters.Count > 0;
            HasResults = results != null && results.Count > 0;
        }

        public void Transform(StringBuilder sb) {
            _context.SetParameter("field", sb.ToString());
            sb.Clear();
            sb.Append(Run());
        }

        public object Transform(object value) {
            _context.SetParameter("field", value);
            return Run();
        }

        public bool HasParameters { get; private set; }
        public bool HasResults { get; private set; }

        private object Run() {
            return _context.Run(_script);
        }

        public void Dispose() {
            _context.Dispose();
        }
    }
}