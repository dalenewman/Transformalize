using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Transformalize.Libs.DBDiff.Schema.SqlServer2005.Generates.Util
{
    public static class Strings
    {
        private const string SPLIT_TITLE_CASE_PATTERN = "(?<!(^|[A-Z]))(?=[A-Z])|(?<!^)(?=[A-Z][a-z])";
        private const string BRACKETS_AND_COMMAS_PATTERN = @"[\[\],]";
        private const string BRACKETS_PATTERN = @"[\[\]]";

        private static readonly Regex TitleCaseRegex = new Regex(SPLIT_TITLE_CASE_PATTERN, RegexOptions.Compiled);
        private static readonly Regex BracketsAndCommasRegex = new Regex(BRACKETS_AND_COMMAS_PATTERN, RegexOptions.Compiled);
        private static readonly Regex BracketsRegex = new Regex(BRACKETS_PATTERN, RegexOptions.Compiled);

        public static string SingleQuotedList(string list, char delimiter)
        {
            var quotedList = string.Empty;
            if (!string.IsNullOrEmpty(list))
            {
                var items = new List<string>(list.Split(delimiter));
                quotedList = String.Join(",", items.Select(db => string.Format("'{0}'", (object) db)).ToArray());
            }
            return quotedList;
        }

        public static string SplitTitleCase(string titleCased, string delimiter)
        {
            return string.Join(delimiter, SplitTitleCase(titleCased).ToArray());
        }

        public static IEnumerable<string> SplitTitleCase(string titleCased)
        {
            return TitleCaseRegex.Split(titleCased).Select(s => s.Trim('_'));
        }

        public static string RemoveBracketsAndCommas(string input)
        {
            return BracketsAndCommasRegex.Replace(input, string.Empty);
        }

        public static string RemoveBrackets(string input)
        {
            return BracketsRegex.Replace(input, string.Empty);
        }

        public static string GetId(string prefix, string content)
        {
            return string.Concat(prefix, "_", content.GetHashCode().ToString(CultureInfo.InvariantCulture).Replace("-", "_"), "_", content.Length.ToString(CultureInfo.InvariantCulture));
        }

    }
}