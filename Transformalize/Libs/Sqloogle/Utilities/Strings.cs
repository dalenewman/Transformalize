using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Transformalize.Libs.Sqloogle.Utilities {
    public static class Strings {
        private const string SPLIT_TITLE_CASE_PATTERN = "(?<!(^|[A-Z]))(?=[A-Z])|(?<!^)(?=[A-Z][a-z])";
        private const string BRACKETS_AND_COMMAS_PATTERN = @"[\[\],]";
        private const string BRACKETS_PATTERN = @"[\[\]]";
        private const string USE_DEFAULT = "0000000000";

        private static readonly Regex TitleCaseRegex = new Regex(SPLIT_TITLE_CASE_PATTERN, RegexOptions.Compiled);
        private static readonly Regex BracketsAndCommasRegex = new Regex(BRACKETS_AND_COMMAS_PATTERN, RegexOptions.Compiled);
        private static readonly Regex BracketsRegex = new Regex(BRACKETS_PATTERN, RegexOptions.Compiled);

        public static string SingleQuotedList(string list, char delimiter) {
            return !string.IsNullOrEmpty(list) ? SingleQuotedList(new List<string>(list.Split(delimiter))) : string.Empty;
        }

        public static string SingleQuotedList(IEnumerable<string> items) {
            return String.Join(",", items.Select(db => string.Format("'{0}'", db)));
        }

        public static string SplitTitleCase(object titleCased, string delimiter) {
            return SplitTitleCase(titleCased.ToString(), delimiter);
        }

        public static string SplitTitleCase(string titleCased, string delimiter) {
            return string.Join(delimiter, SplitTitleCase(titleCased));
        }

        public static IEnumerable<string> SplitTitleCase(string titleCased) {
            return TitleCaseRegex.Split(titleCased).Select(s => s.Trim('_'));
        }

        public static string RemoveBracketsAndCommas(string input) {
            return BracketsAndCommasRegex.Replace(input, string.Empty);
        }

        public static string RemoveBrackets(string input) {
            return BracketsRegex.Replace(input, string.Empty);
        }

        public static string UseBucket(object number, int length = 10, char padChar = '0') {
            if (number == null)
                return USE_DEFAULT;
            var useString = number.ToString();
            return useString.Equals(string.Empty) ? USE_DEFAULT : (useString[0] + new string(padChar, useString.Length - 1)).PadLeft(length, padChar);
        }

    }
}