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
using System.Collections.Generic;
using System.Linq;

namespace Pipeline.Extensions {

    public static class StringExtensions {
        public static string Left(this string s, int length) {
            return s.Length > length ? s.Substring(0, length) : s;
        }

        public static string Right(this string s, int length) {
            return s.Length > length ? s.Substring(s.Length - length, length) : s;
        }

        public static bool IsNumeric(this string value) {
            double retNum;
            return double.TryParse(value, System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out retNum);
        }

        public static bool In(this string value, params string[] args) {
            if (args == null || args.Length == 0)
                return false;
            return args.Any(s => s == value);
        }

        /// <summary>
        /// Credit to Marc Cliftlon
        /// http://www.marcclifton.com/
        /// http://www.codeproject.com/Members/Marc-Clifton
        /// </summary> 
        public static string[] DelimiterSplit(this string src, char delimeter, char quote = '\"') {
            var result = new List<string>();
            var index = 0;
            var start = 0;
            var inQuote = false;

            while (index < src.Length) {
                if ((!inQuote) && (src[index] == delimeter)) {
                    result.Add(src.Substring(start, index - start).Trim());
                    start = index + 1;		// Ignore the delimiter.
                }

                if (src[index] == quote) {
                    inQuote = !inQuote;
                }

                ++index;
            }

            // The last part.
            if (!inQuote) {
                result.Add(src.Substring(start, index - start).Trim());
            }
            return result.ToArray();
        }

        public static IEnumerable<string> SplitLine(this string line, char delimiter, bool quoted) {
            return quoted ? line.DelimiterSplit(delimiter) : line.Split(delimiter);
        }


    }
}