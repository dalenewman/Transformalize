using System.Linq;
using System.Text;

namespace Transformalize {

    public static class StringBuilderExtensions {

        public static StringBuilder TrimStart(this StringBuilder sb, string trimChars) {
            var length = sb.Length;
            if (length != 0) {
                var i = 0;

                if (trimChars.Length == 1) {
                    while (i < length && sb[i] == trimChars[0]) {
                        i++;
                    }
                }
                else {
                    while (i < length && trimChars.Any(c => c.Equals(sb[i]))) {
                        i++;
                    }
                }

                if (i > 0) {
                    sb.Remove(0, i);
                }
            }
            return sb;
        }

        public static StringBuilder TrimEnd(this StringBuilder sb, string trimChars) {
            var length = sb.Length;
            if (length != 0) {
                var i = length - 1;

                if (trimChars.Length == 1) {
                    while (i > -1 && sb[i] == trimChars[0]) {
                        i--;
                    }
                }
                else {
                    while (i > -1 && trimChars.Any(c => c.Equals(sb[i]))) {
                        i--;
                    }
                }

                if (i < (length - 1)) {
                    sb.Remove(i + 1, (length - i) - 1);
                }
            }
            return sb;
        }

        public static StringBuilder Trim(this StringBuilder sb, string trimChars) {
            return sb.TrimStart(trimChars).TrimEnd(trimChars);
        }

        public static StringBuilder Substring(this StringBuilder sb, int startIndex, int length) {
            var capacity = sb.Length;
            if (capacity != 0 && startIndex < capacity) {

                if (startIndex + length > capacity) {
                    length = capacity - startIndex;
                }

                sb.Remove(startIndex + length, capacity - (startIndex + length));
                sb.Remove(0, startIndex);

            }
            return sb;
        }

        public static StringBuilder Left(this StringBuilder sb, int length) {
            return sb.Substring(0, length);
        }

        public static StringBuilder Right(this StringBuilder sb, int length) {
            return sb.Substring(sb.Length - length, length);
        }

        public static bool IsEqualTo(this StringBuilder sb, string value) {
            var valLen = value.Length;
            if (sb.Length == valLen) {
                for (var i = 0; i < valLen; i++) {
                    if (!sb[i].Equals(value[i])) {
                        break;
                    }
                    if (i == valLen - 1) {
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool StartsWith(this StringBuilder sb, string value) {
            var valLen = value.Length;
            if (valLen < sb.Length) {
                for (var i = 0; i < valLen; i++) {
                    if (!sb[i].Equals(value[i])) {
                        break;
                    }
                    if (i == valLen - 1) {
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool EndsWith(this StringBuilder sb, string value) {
            var sbLen = sb.Length;
            var valLen = value.Length;
            if (valLen < sbLen) {
                for (var i = 0; i < valLen; i++) {
                    if (!value[i].Equals(sb[sbLen - valLen + i])) {
                        break;
                    }
                    if (i == valLen - 1) {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
