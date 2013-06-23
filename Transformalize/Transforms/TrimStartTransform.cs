using System.Text;

namespace Transformalize.Transforms
{
    public class TrimStartTransform : ITransform {
        private readonly string _trimChars;

        public TrimStartTransform(string trimChars) {
            _trimChars = trimChars;
        }

        public void Transform(StringBuilder sb) {
            sb.TrimStart(_trimChars);
        }
    }
}