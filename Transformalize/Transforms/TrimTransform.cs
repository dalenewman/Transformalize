using System;
using System.Text;

namespace Transformalize.Transforms {
    public class TrimTransform : ITransform, IDisposable {
        private readonly string _trimChars;
        private readonly char[] _trimCharArray;

        public TrimTransform(string trimChars) {
            _trimChars = trimChars;
            _trimCharArray = trimChars.ToCharArray();
        }

        public void Transform(StringBuilder sb) {
            sb.Trim(_trimChars);
        }

        public object Transform(object value) {
            return value.ToString().Trim(_trimCharArray);
        }

        public void Dispose() { }
    }
}