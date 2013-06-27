using System;
using System.Text;

namespace Transformalize.Transforms
{
    public class TrimStartTransform : ITransform, IDisposable {
        private readonly string _trimChars;
        private readonly char[] _trimCharArray;

        public TrimStartTransform(string trimChars) {
            _trimChars = trimChars;
            _trimCharArray = trimChars.ToCharArray();
        }

        public void Transform(StringBuilder sb) {
            sb.TrimStart(_trimChars);
        }

        public object Transform(object value)
        {
            return value.ToString().TrimStart(_trimCharArray);
        }

        public void Dispose() { }
    }
}