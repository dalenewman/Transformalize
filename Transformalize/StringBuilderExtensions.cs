﻿using System.Linq;
using System.Text;

namespace Transformalize {

    public static class StringBuilderExtensions {

        public static StringBuilder TrimStart(this StringBuilder sb, string trimChars) {
            var length = sb.Length;
            if (length != 0) {
                var i = 0;

                if (trimChars.Length == 1) {
                    while (sb[i] == trimChars[0] && i < length) {
                        i++;
                    }
                }
                else {
                    while (trimChars.Any(c => c.Equals(sb[i])) && i < length) {
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
                    while (sb[i] == trimChars[0] && i > -1) {
                        i--;
                    }
                } else {
                    while (trimChars.Any(c => c.Equals(sb[i])) && i > -1) {
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

    }
}