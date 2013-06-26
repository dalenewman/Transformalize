using System.Text;

namespace Transformalize.Transforms
{
    public class LeftTransform : ITransform {
        private readonly int _length;

        public LeftTransform(int length) {
            _length = length;
        }

        public void Transform(StringBuilder sb) {
            sb.Left(_length);
        }
    }
}