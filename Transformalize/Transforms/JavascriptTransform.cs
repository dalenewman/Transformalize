using System;
using System.Text;
using Noesis.Javascript;

namespace Transformalize.Transforms {
    public class JavascriptTransform : ITransform, IDisposable {
        private readonly JavascriptContext _context = new JavascriptContext();
        private readonly string _script;

        public JavascriptTransform(string script) {
            _script = script;
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

        private object Run() {
            return _context.Run(_script);
        }

        public void Dispose() {
            _context.Dispose();
        }
    }
}