using System.Text;

namespace Transformalize.Transforms
{
    public class RightTransform : ITransform {
        private readonly int _length;

        public RightTransform(int length) {
            _length = length;
        }

        public void Transform(StringBuilder sb) {
            sb.Right(_length);
        }
    }
}