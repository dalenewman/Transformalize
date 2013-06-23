using System.Text;

namespace Transformalize.Transforms
{
    public class TrimTransform : ITransform {
        private readonly string _trimChars;

        public TrimTransform(string trimChars) {
            _trimChars = trimChars;
        }

        public void Transform(StringBuilder sb) {
            sb.Trim(_trimChars);
        }
    }
}