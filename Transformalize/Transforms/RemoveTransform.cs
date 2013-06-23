using System.Text;

namespace Transformalize.Transforms
{
    public class RemoveTransform : ITransform {
        private readonly int _startIndex;
        private readonly int _length;

        public RemoveTransform(int startIndex, int length) {
            _startIndex = startIndex;
            _length = length;
        }

        public void Transform(StringBuilder sb) {
            sb.Remove(_startIndex, _length);
        }
    }
}