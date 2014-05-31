using System.Collections.Generic;

namespace Transformalize.Extensions {

    /// <summary>
    /// Credit to Marc Cliftlon
    /// http://www.marcclifton.com/
    /// http://www.codeproject.com/Members/Marc-Clifton
    /// </summary> 
    public static class ListExtension {

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
    }
}