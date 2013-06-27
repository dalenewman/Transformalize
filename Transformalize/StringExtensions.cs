using System.Linq;
using System.Text;

namespace Transformalize {

    public static class StringExtensions {

        public static string Left(this string s, int length) {
            return s.Substring(0, length);
        }

        public static string Right(this string s, int length) {
            return s.Substring(s.Length - length, length);
        }

    }
}
