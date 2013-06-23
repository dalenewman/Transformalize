using System.Text;

namespace Transformalize.Transforms
{
    public class TrimEndTransform : ITransform {
        private readonly string _trimChars;

        public TrimEndTransform(string trimChars) {
            _trimChars = trimChars;
        }

        public void Transform(StringBuilder sb) {
            sb.TrimEnd(_trimChars);
        }
    }
}