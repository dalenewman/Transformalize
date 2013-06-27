using System;
using System.Text;

namespace Transformalize.Transforms {
    public class RightTransform : ITransform, IDisposable {
        private readonly int _length;

        public RightTransform(int length) {
            _length = length;
        }

        public void Transform(StringBuilder sb) {
            sb.Right(_length);
        }

        public object Transform(object value) {
            return value.ToString().Right(_length);
        }

        public void Dispose() { }
    }
}