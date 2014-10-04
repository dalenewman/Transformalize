using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Libs.Sqloogle.Utilities;

namespace Transformalize.Libs.Sqloogle.Operations {

    public class CachedSqlPreTransform : AbstractOperation {

        private const StringComparison IGNORE = StringComparison.OrdinalIgnoreCase;

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows.AsParallel()) {

                var sql = Trim(row["sql"]);

                if (sql.StartsWith("/* SQLoogle */")) continue;
                if (!IsOfInterest(sql)) continue;

                sql = ReplaceParameters(sql);
                row["sql"] = RemoveOptionalDetails(sql);
                row["database"] = row["database"] ?? string.Empty;
                yield return row;
            }
        }

        /// <summary>
        /// removes extra brackets, and double-quotes around object names.  removes default schemas and unnecessary semi-colons.
        /// </summary>
        public static string RemoveOptionalDetails(string sql) {

            var result = sql;
            var names = (from Match match in SqlStrings.MatchSqlNameOptionals(sql) select match.Value).ToList();

            if (names.Any()) {
                if (names.Count > 2) {
                    var sb = new StringBuilder(sql);
                    foreach (var name in names.Distinct())
                        sb.Replace(name, name.Trim("[]\"".ToCharArray()));
                    result = sb.ToString();
                }
                else {
                    foreach (var name in names.Distinct())
                        result = sql.Replace(name, name.Trim("[]\"".ToCharArray()));
                }
            }

            return SqlStrings.RemoveSqlOptionals(result);
        }

        /// <summary>
        /// trims extra carraige returns, new lines, tabs, and spaces
        /// </summary>
        public static string Trim(object sqlObject) {
            var sql = sqlObject.ToString();
            return string.IsNullOrEmpty(sql) ? string.Empty : sql.Trim("\r\n\t; ".ToCharArray());
        }

        public static int Lines(string sql, bool trimFirst = true) {
            if (trimFirst)
                sql = Trim(sql);
            return string.IsNullOrEmpty(sql) ? 0 : sql.ToCharArray().Count(c => c == '\n') + 1;
        }

        public static int Statements(string sql, bool trimFirst = true) {
            if (trimFirst)
                sql = Trim(sql);
            return string.IsNullOrEmpty(sql) ? 0 : sql.ToCharArray().Count(c => c == ';') + 1;
        }

        /// <summary>
        /// detects "noise" queries; single-line queries we're not interested in.
        /// </summary>
        public static bool IsNoise(string sql) {
            sql = Trim(SqlStrings.RemoveSqlComments(sql));
            if (Lines(sql, false) == 1) {
                if (Statements(sql, false) == 1) {
                    return (sql.StartsWith("FETCH ", IGNORE) || sql.StartsWith("SET ", IGNORE) || sql.Equals("begin transaction", IGNORE) || sql.Equals("commit transaction", IGNORE));
                }
            }
            return false;
        }

        public static string RemoveExtraDoubleQuotesAndBrackets(string sql) {
            var matches = SqlStrings.MatchSqlExtras(sql);
            var names = (from Match match in matches select match.Value).ToList();
            foreach (var name in names.Distinct()) {
                sql = sql.Replace(name, name.Trim("[]\"".ToCharArray()));
            }
            return sql;
        }

        public static IEnumerable<string> FindTables(string sql) {

            sql = RemoveExtraDoubleQuotesAndBrackets(sql);
            var tables = new List<string>();
            var matches = SqlStrings.MatchSqlTables(sql);

            foreach (Match match in matches) {
                var table = match.Groups["table"].Value;
                tables.Add(table);
            }

            return tables.Distinct();
        }

        public static List<string> GetItems(string sql, int position, string defaultValue) {
            var items = new List<string>();
            var tables = FindTables(sql);
            var hasItem = false;

            foreach (string table in tables) {
                var item = defaultValue;
                var elements = table.Split('.');

                if (elements.Length > position) {
                    item = elements[elements.Length - (position + 1)];
                }

                if (item != null) {
                    items.Add(item.Trim("[]\"".ToCharArray()));
                    hasItem = true;
                }

            }

            return hasItem ? items.Distinct().ToList() : new List<string>();
        }

        public static List<string> FindSchemas(string sql) {
            return GetItems(sql, 1, "dbo");
        }

        public static List<string> FindDatabases(string sql) {
            return GetItems(sql, 2, null);
        }

        public static string FindCommonSchema(string sql) {
            var schemas = FindSchemas(sql);
            return schemas.Count() == 1 ? schemas.ElementAt(0) : string.Empty;
        }

        public static string FindCommonDatabase(string sql) {
            var databases = FindDatabases(sql);
            return databases.Count() == 1 ? databases.ElementAt(0) : string.Empty;
        }

        public static bool IsOfInterest(string sql) {

            if (!IsNoise(sql)) {
                var schema = FindCommonSchema(sql);
                return !schema.Equals("sys", IGNORE);
                //if (!schema.Equals("sys", IGNORE)) {
                //    var database = FindCommonDatabase(sql);
                //    if (string.IsNullOrEmpty(database))
                //        return true;
                //    return !config.Skips.Match(database);
                //}
            }
            return false;
        }

        public static string ReplaceParameters(string sql) {
            sql = SqlStrings.ReplaceSqlEmptySingleQuotes(sql, "1");
            sql = SqlStrings.ReplaceSqlColumnOperatorValues(sql, "${operator} @Parameter");
            return sql; //SqlStrings.ReplaceSqlParametersInFunctions(sql, "@Parameter");
        }    
    }
}
