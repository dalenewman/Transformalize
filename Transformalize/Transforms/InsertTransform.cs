using System.Text;

namespace Transformalize.Transforms
{
    public class InsertTransform : ITransform {
        private readonly int _index;
        private readonly string _value;

        public InsertTransform(int index, string value) {
            _index = index;
            _value = value;
        }

        public void Transform(StringBuilder sb) {
            sb.Insert(_index, _value);
        }
    }
}