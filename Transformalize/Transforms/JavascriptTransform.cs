using System.Collections.Generic;
using System.Text;
using Noesis.Javascript;
using Transformalize.Model;
using Transformalize.Rhino.Etl.Core;

namespace Transformalize.Transforms {
    public class JavascriptTransform : ITransform {
        private readonly JavascriptContext _context = new JavascriptContext();
        private readonly string _script;
        public Dictionary<string, Field> Parameters { get; private set; }
        public Dictionary<string, Field> Results { get; private set; }
        private readonly bool _hasParameters;
        private readonly bool _hasResults;

        public bool HasParameters { get { return _hasParameters; } }
        public bool HasResults { get { return _hasResults; } }

        public JavascriptTransform(string script) {
            _script = script;
        }

        public JavascriptTransform(string script, Dictionary<string, Field> parameters, Dictionary<string, Field> results) {
            _script = script;
            Parameters = parameters;
            Results = results;
            _hasParameters = parameters != null && parameters.Count > 0;
            _hasResults = results != null && results.Count > 0;
        }

        public void Transform(ref StringBuilder sb) {
            _context.SetParameter("field", sb.ToString());
            sb.Clear();
            sb.Append(Run());
        }

        public void Transform(ref object value) {
            _context.SetParameter("field", value);
            value = Run();
        }

        public void Transform(ref Row row) {
            foreach (var key in Parameters.Keys) {
                _context.SetParameter(key, row[key]);
            }
            var result = Run();
            foreach (var key in Results.Keys) {
                row[key] = result;
            }
        }

        private object Run() {
            return _context.Run(_script);
        }

        public void Dispose() {
            _context.Dispose();
        }
    }
}