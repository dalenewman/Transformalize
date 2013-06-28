using System.Text;

namespace Transformalize.Transforms {
    public class PadRightTransform : ITransform {
        private readonly int _totalWidth;
        private readonly char _paddingChar;

        public PadRightTransform(int totalWidth, char paddingChar) {
            _totalWidth = totalWidth;
            _paddingChar = paddingChar;
        }

        public void Transform(StringBuilder sb) {
            sb.PadRight(_totalWidth, _paddingChar);
        }

        public object Transform(object value) {
            return value.ToString().PadRight(_totalWidth, _paddingChar);
        }

        public void Dispose() { }


    }
}