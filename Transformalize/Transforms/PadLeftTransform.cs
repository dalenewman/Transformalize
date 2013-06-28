using System.Text;

namespace Transformalize.Transforms {
    public class PadLeftTransform : ITransform {
        private readonly int _totalWidth;
        private readonly char _paddingChar;

        public PadLeftTransform(int totalWidth, char paddingChar) {
            _totalWidth = totalWidth;
            _paddingChar = paddingChar;
        }

        public void Transform(StringBuilder sb) {
            sb.PadLeft(_totalWidth, _paddingChar);
        }

        public object Transform(object value) {
            return value.ToString().PadLeft(_totalWidth, _paddingChar);
        }

        public void Dispose() { }
    }
}