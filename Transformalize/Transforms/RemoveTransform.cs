using System;
using System.Text;

namespace Transformalize.Transforms {
    public class RemoveTransform : ITransform, IDisposable {
        private readonly int _startIndex;
        private readonly int _length;

        public RemoveTransform(int startIndex, int length) {
            _startIndex = startIndex;
            _length = length;
        }

        public void Transform(StringBuilder sb) {
            sb.Remove(_startIndex, _length);
        }

        public object Transform(object value) {
            return value.ToString().Remove(_startIndex, _length);
        }

        public void Dispose() { }
    }
}