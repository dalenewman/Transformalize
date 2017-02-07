#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2017 Dale Newman
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//   
//       http://www.apache.org/licenses/LICENSE-2.0
//   
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Transformalize.Extensions {

    public static class StringBuilderExtensions {

        public static int LastIndexOf(this StringBuilder sb, char value) {
            for (var i = sb.Length - 1; i > -1; i--) {
                if (sb[i].Equals(value)) {
                    return i;
                }
            }
            return -1;
        }

        public static void RemoveNonNumeric(this StringBuilder sb) {
            var numbers = new List<char>();
            var found = false;
            for (var i = 0; i < sb.Length; i++) {
                if (!char.IsNumber(sb[i])) continue;
                found = true;
                numbers.Add(sb[i]);
            }
            if (!found) return;
            sb.Clear();
            sb.Append(numbers.ToArray());
        }

        public static void InsertFormat(this StringBuilder sb, int index, string format, params object[] args) {
            var length = sb.Length;

            if (length > index) {
                var keep = new Stack<char>();
                for (var i = length - 1; i >= index; i--) {
                    keep.Push(sb[i]);
                }
                sb.Remove(index, length - index);
                sb.AppendFormat(format, args);
                if (!keep.Any()) return;
                while (keep.Count > 0) {
                    sb.Append(keep.Pop());
                }
            } else {
                sb.AppendFormat(format, args);
            }
        }

        public static void TrimStart(this StringBuilder sb, string trimChars) {
            var length = sb.Length;
            if (length != 0) {
                var i = 0;
                var chars = trimChars.ToCharArray();

                if (chars.Length == 1) {
                    while (i < length && sb[i] == trimChars[0]) {
                        i++;
                    }
                } else {
                    while (i < length && chars.Any(c => c.Equals(sb[i]))) {
                        i++;
                    }
                }

                if (i > 0) {
                    sb.Remove(0, i);
                }
            }
        }

        public static void TrimEnd(this StringBuilder sb, string trimChars) {
            var length = sb.Length;
            if (length != 0) {
                var chars = trimChars.ToCharArray();
                var i = length - 1;

                if (trimChars.Length == 1) {
                    while (i > -1 && sb[i] == trimChars[0]) {
                        i--;
                    }
                } else {
                    while (i > -1 && chars.Any(c => c.Equals(sb[i]))) {
                        i--;
                    }
                }

                if (i < (length - 1)) {
                    sb.Remove(i + 1, (length - i) - 1);
                }
            }
        }

        public static void Trim(this StringBuilder sb, string trimChars) {
            sb.TrimStart(trimChars);
            sb.TrimEnd(trimChars);
        }

        public static void Trim(this StringBuilder sb, params char[] chars) {
            sb.Trim(string.Concat(chars));
        }

        public static void Substring(this StringBuilder sb, int startIndex, int length) {
            var capacity = sb.Length;
            if (capacity != 0 && startIndex < capacity) {
                if (startIndex + length > capacity) {
                    length = capacity - startIndex;
                }

                sb.Remove(startIndex + length, capacity - (startIndex + length));
                sb.Remove(0, startIndex);
            }
        }

        public static void PadLeft(this StringBuilder sb, int totalWidth, char paddingChar) {
            var sbLen = sb.Length;
            if (sbLen < totalWidth) {
                for (var i = 0; i < totalWidth - sbLen; i++) {
                    sb.Insert(0, new[] { paddingChar });
                }
            }
        }

        public static void PadRight(this StringBuilder sb, int totalWidth, char paddingChar) {
            var sbLen = sb.Length;
            if (sbLen < totalWidth) {
                for (var i = 0; i < totalWidth - sbLen; i++) {
                    sb.Append(paddingChar);
                }
            }
        }

        public static void Left(this StringBuilder sb, int length) {
            sb.Substring(0, length);
        }

        public static void Right(this StringBuilder sb, int length) {
            sb.Substring(sb.Length - length, length);
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

        public static void Concat(this StringBuilder sb, params object[] args) {
            foreach (var obj in args) {
                sb.Append(obj);
            }
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

        public static void ToLower(this StringBuilder sb) {
            for (var i = 0; i < sb.Length; i++) {
                if (char.IsUpper(sb[i])) {
                    sb[i] = char.ToLower(sb[i]);
                }
            }
        }

        public static void ToUpper(this StringBuilder sb) {
            for (var i = 0; i < sb.Length; i++) {
                if (char.IsLower(sb[i])) {
                    sb[i] = char.ToUpper(sb[i]);
                }
            }
        }

        public static void Push(this StringBuilder sb, Func<char, bool> selector) {
            var count = 0;
            var numbers = new char[sb.Length];
            for (var i = 0; i < sb.Length; i++) {
                if (selector(sb[i])) {
                    count++;
                    numbers[i] = sb[i];
                } else {
                    break;
                }
            }
            if (count <= 0)
                return;

            sb.Remove(0, count);

            for (var i = 0; i < numbers.Length; i++) {
                if (numbers[i].Equals(default(char))) {
                    break;
                }
                sb.Append(numbers[i]);
            }
        }
    }
}