using System;
using System.Text;

namespace Transformalize.Transforms {
    public class ReplaceTransform : ITransform, IDisposable {

        private readonly string _oldValue;
        private readonly string _newValue;

        public ReplaceTransform(string oldValue, string newValue) {
            _oldValue = oldValue;
            _newValue = newValue;
        }

        public void Transform(StringBuilder sb) {
            sb.Replace(_oldValue, _newValue);
        }

        public object Transform(object value) {
            return value.ToString().Replace(_oldValue, _newValue);
        }

        public void Dispose() { }
    }
}