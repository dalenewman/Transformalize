#region license
// Transformalize
// A Configurable ETL solution specializing in incremental denormalization.
// Copyright 2013 Dale Newman
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
using System.Reflection;
using System.Text.RegularExpressions;
using Pipeline.Configuration;
using Pipeline.Extensions;

namespace Pipeline {

    public static class Utility {

        internal static string ControlString = ((char)31).ToString();
        internal static char ControlChar = (char)31;

        /// <summary>
        /// Splits a sting by a splitter (aka delimiter), 
        /// but first escapes any splitters prefixed with a forward slash.
        /// </summary>
        /// <param name="arg">arguments</param>
        /// <param name="splitter">the splitter (aka delimiter)</param>
        /// <param name="skip">An optional number of post-split elements to skip over.</param>
        /// <returns>properly split strings</returns>
        public static string[] Split(string arg, char splitter, int skip = 0) {
            if (arg.Equals(string.Empty))
                return new string[0];

            var split = arg.Replace("\\" + splitter, ControlString).Split(splitter);
            return
                split.Select(s => s.Replace(ControlChar, splitter))
                    .Skip(skip)
                    .Where(s => !string.IsNullOrEmpty(s))
                    .ToArray();
        }

        public static string[] Split(string arg, string[] splitter, int skip = 0) {
            if (arg.Equals(string.Empty))
                return new string[0];

            var split = arg.Replace("\\" + splitter[0], ControlString).Split(splitter, StringSplitOptions.None);
            return
                split.Select(s => s.Replace(ControlString, splitter[0]))
                    .Skip(skip)
                    .Where(s => !string.IsNullOrEmpty(s))
                    .ToArray();
        }

        private static readonly Dictionary<string, Func<object, object, bool>> CompareMap = new Dictionary<string, Func<object, object, bool>>() {
            {"=", ((x, y) => x.Equals(y))},
            {"==", ((x, y) => x.Equals(y))},
            {"equal", ((x, y) => x.Equals(y))},
            {"!=", ((x, y) => !x.Equals(y))},
            {"notequal", ((x, y) => !x.Equals(y))},
            {">", ((x, y) => ((IComparable) x).CompareTo(y) > 0)},
            {"greaterthan", ((x, y) => ((IComparable) x).CompareTo(y) > 0)},
            {">=", ((x, y) => x.Equals(y) || ((IComparable) x).CompareTo(y) > 0)},
            {"greaterthanequal", ((x, y) => x.Equals(y) || ((IComparable) x).CompareTo(y) > 0)},
            {"<", ((x, y) => ((IComparable) x).CompareTo(y) < 0)},
            {"lessthan", ((x, y) => ((IComparable) x).CompareTo(y) < 0)},
            {"<=", ((x, y) => x.Equals(y) || ((IComparable) x).CompareTo(y) < 0)},
            {"lessthanequal", ((x, y) => x.Equals(y) || ((IComparable) x).CompareTo(y) < 0)}
        };

        public static bool Evaluate(object left, string @operator, object right) {
            return CompareMap[@operator](left, right);
        }

        public static string Identifier(string name, string substitute = "_") {
            var first = Regex.Replace(name, @"^[0-9]{1}|\W", substitute).TrimEnd(substitute.ToCharArray()).Left(128);
            return Regex.Replace(first, substitute+"{2,}", substitute);
        }

        public static string GetExcelName(int index) {
            var name = Convert.ToString((char)('A' + (index % 26)));
            while (index >= 26) {
                index = (index / 26) - 1;
                name = Convert.ToString((char)('A' + (index % 26))) + name;
            }
            return name;
        }

        public static byte[] HexStringToByteArray(string hex) {
            var bytes = new byte[hex.Length / 2];
            var hexValue = new[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F };

            for (int x = 0, i = 0; i < hex.Length; i += 2, x += 1) {
                bytes[x] = (byte)(hexValue[char.ToUpper(hex[i + 0]) - '0'] << 4 |
                                  hexValue[char.ToUpper(hex[i + 1]) - '0']);
            }
            return bytes;
        }

        public static string BytesToHexString(byte[] bytes) {
            var c = new char[bytes.Length * 2];
            for (var i = 0; i < bytes.Length; i++) {
                var b = bytes[i] >> 4;
                c[i * 2] = (char)(55 + b + (((b - 10) >> 31) & -7));
                b = bytes[i] & 0xF;
                c[i * 2 + 1] = (char)(55 + b + (((b - 10) >> 31) & -7));
            }
            return new string(c);
        }

        public static char FindDelimiter(IEnumerable<string> strings, List<Delimiter> delimiters, bool quoted) {


            var lines = strings.ToArray();

            if (!lines.Any()) {
                return default(char);
            }

            var isSample = lines.Count() == 100;
            var count = Convert.ToDouble(lines.Count());

            var delimiterCounts = new Dictionary<Delimiter, List<int>>();

            foreach (var d in delimiters) {
                delimiterCounts[d] = new List<int>();
                foreach (var line in lines) {
                    delimiterCounts[d].Add(line.SplitLine(d.Character, quoted).Count() - 1);
                }
                var average = delimiterCounts[d].Average();
                var min = delimiterCounts[d].Min();
                if (min == 0 || !(average > 0))
                    continue;

                var variance = delimiterCounts[d].Sum(l => Math.Pow((l - average), 2)) / (count - (isSample ? 1 : 0));

                d.AveragePerLine = average;
                d.StandardDeviation = lines.Count() == 1 ? 0 : Math.Sqrt(variance);
            }

            var winner = delimiters
                .Where(d => d.AveragePerLine > 0)
                .OrderBy(d => d.CoefficientOfVariance())
                .ThenByDescending(d => d.AveragePerLine)
                .FirstOrDefault();

            return winner?.Character ?? default(char);
        }
        public static object GetPropValue(object src, string propName) {
            return src.GetType().GetRuntimeProperty(propName).GetValue(src);
        }

    }
}
