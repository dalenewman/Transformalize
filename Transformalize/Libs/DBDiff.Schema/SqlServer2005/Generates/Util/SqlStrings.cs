using System;
using System.Text.RegularExpressions;

namespace Transformalize.Libs.DBDiff.Schema.SqlServer2005.Generates.Util
{
    public static class SqlStrings
    {
        private const string SQL_PUNCTUATION_PATTERN = @"[\.\[\];\t\n""]";
        private const string SQL_COMMENTS_PATTERN = @"(/\*([^*]|[\r\n]|(\*+([^*/]|[\r\n])))*\*+/)|(--.*$)";
        private const string SQL_OPTIONAL_PATTERN = @"((?<=\s)dbo\.|;(?=\r\n)|;$)";
        private const string SQL_OPTIONAL_NAME_PATTERN = @"[\[""]{1}\w+[^\s]\w+[\]""]{1}";
        private const string SQL_EMPTY_SINGLE_QUOTED_STRING_PATTERN = @"(?<=[^'])'{2}(?=[^'])";
        private const string SQL_COLUMN_OPERATOR_VALUE_PATTERN = @"(?<column>[""\[]?\w+[""\]]?)\s*(?<operator>[=<>!]{1,2}|not like|like)\s*(?<value>(\-?[\d\.]+|N?'[^']*'|0x[0-9a-fA-F]+))((?=\W)|\z)";
        private const string SQL_PARAMETER_IN_FUNCTION_PATTERN = @"(?<=\(.*)(?<value>\b\-?([\d\.]{2,}|[\d]+)\b|N?'[^']*'|0x[0-9a-fA-F]+)(?=.*\))";
        private const string SQL_TABLE_PATTERN = @"(?<=(?<keywords>update|into|from|join|insert)\s+)(?<table>[\[""][\w\s\.\[\]""]+[\]""](?=([^\.]|$))|[\w\.\[\]""]+)";
        private const string SQL_EXTRAS_PATTERN = @"[\[""]{1}\w+[^\s]\w+[\]""]{1}";
        private const string HEX_ENDING_PATTERN = "__[0-9A-F]{8}$";
        private const string BRACKETS_PATTERN = @"[\[\]]";

        private const RegexOptions COMMON_OPTIONS = RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled;

        private static readonly Regex SqlPunctuationRegex = new Regex(SQL_PUNCTUATION_PATTERN, RegexOptions.Compiled);
        private static readonly Regex SqlCommentsRegex = new Regex(SQL_COMMENTS_PATTERN, COMMON_OPTIONS);
        private static readonly Regex SqlOptionalRegex = new Regex(SQL_OPTIONAL_PATTERN, COMMON_OPTIONS);
        private static readonly Regex SqlNameOptionalRegex = new Regex(SQL_OPTIONAL_NAME_PATTERN, COMMON_OPTIONS);
        private static readonly Regex SqlEmptySingleQuotedRegex = new Regex(SQL_EMPTY_SINGLE_QUOTED_STRING_PATTERN, COMMON_OPTIONS);
        private static readonly Regex SqlColumnOperatorValueRegex = new Regex(SQL_COLUMN_OPERATOR_VALUE_PATTERN, COMMON_OPTIONS);
        private static readonly Regex SqlParameterInFunctionRegex = new Regex(SQL_PARAMETER_IN_FUNCTION_PATTERN, COMMON_OPTIONS);
        private static readonly Regex SqlTableRegex = new Regex(SQL_TABLE_PATTERN, COMMON_OPTIONS);
        private static readonly Regex SqlExtrasRegex = new Regex(SQL_EXTRAS_PATTERN, COMMON_OPTIONS);
        private static readonly Regex HexEndingRegex = new Regex(HEX_ENDING_PATTERN, RegexOptions.Compiled);
        private static readonly Regex BracketsRegex = new Regex(BRACKETS_PATTERN, RegexOptions.Compiled);
        
        public static string RemoveSqlPunctuation(string sql)
        {
            return SqlPunctuationRegex.Replace(sql, " ");
        }

        public static string RemoveSqlComments(string sql)
        {
            return SqlCommentsRegex.Replace(sql, String.Empty);
        }

        public static string RemoveSqlOptionals(string sql)
        {
            return SqlOptionalRegex.Replace(sql, String.Empty);
        }

        public static MatchCollection MatchSqlNameOptionals(string sql)
        {
            return SqlNameOptionalRegex.Matches(sql);
        }

        public static MatchCollection MatchSqlTables(string sql)
        {
            return SqlTableRegex.Matches(sql);
        }

        public static MatchCollection MatchSqlExtras(string sql)
        {
            return SqlExtrasRegex.Matches(sql);
        }

        public static string ReplaceSqlEmptySingleQuotes(string sql, string replacement)
        {
            return SqlEmptySingleQuotedRegex.Replace(sql, replacement);
        }

        public static string ReplaceSqlColumnOperatorValues(string sql, string replacement)
        {
            return SqlColumnOperatorValueRegex.Replace(sql, replacement);
        }

        public static string ReplaceSqlParametersInFunctions(string sql, string replacement)
        {
            return SqlParameterInFunctionRegex.Replace(sql, replacement);
        }

        public static bool HasHexEnding(string sqlName)
        {
            return HexEndingRegex.IsMatch(sqlName);
        }

        public static string RemoveBrackets(string input)
        {
            return BracketsRegex.Replace(input, string.Empty);
        }
        
    }
}