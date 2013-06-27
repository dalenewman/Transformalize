using System;
using System.Text;

namespace Transformalize.Transforms {
    public class SubstringTransform : ITransform, IDisposable {
        private readonly int _startIndex;
        private readonly int _length;

        public SubstringTransform(int startIndex, int length) {
            _startIndex = startIndex;
            _length = length;
        }

        public void Transform(StringBuilder sb) {
            sb.Substring(_startIndex, _length);
        }

        public object Transform(object value) {
            return value.ToString().Substring(_startIndex, _length);
        }

        public void Dispose() { }
    }
}