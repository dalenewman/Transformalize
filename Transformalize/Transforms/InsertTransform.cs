using System;
using System.Text;

namespace Transformalize.Transforms {
    public class InsertTransform : ITransform, IDisposable {
        private readonly int _index;
        private readonly string _value;

        public InsertTransform(int index, string value) {
            _index = index;
            _value = value;
        }

        public void Transform(StringBuilder sb) {
            sb.Insert(_index, _value);
        }

        public object Transform(object value) {
            return value.ToString().Insert(_index, _value);
        }

        public void Dispose() { }
    }
}