using System;
using System.Text;

namespace Transformalize.Transforms {
    public class TrimEndTransform : ITransform, IDisposable {
        private readonly string _trimChars;
        private readonly char[] _trimCharArray;

        public TrimEndTransform(string trimChars) {
            _trimChars = trimChars;
            _trimCharArray = trimChars.ToCharArray();
        }

        public void Transform(StringBuilder sb) {
            sb.TrimEnd(_trimChars);
        }

        public object Transform(object value) {
            return value.ToString().TrimEnd(_trimCharArray);
        }

        public void Dispose() { }


    }
}