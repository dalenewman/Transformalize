using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;

namespace Transformalize.Libs.Sqloogle.Operations {

    public class SqloogleTransform : AbstractOperation {

        private const RegexOptions OPTIONS = RegexOptions.Compiled;

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {

            foreach (var row in rows) {
                row["created"] = row["created"];
                row["modified"] = row["modified"];
                row["lastused"] = row["lastused"];
                row["server"] = ListTransform(row["server"]);
                row["database"] = ListTransform(row["database"]);
                row["schema"] = ListTransform(row["schema"]);
                row["name"] = ListTransform(row["name"]);
                row["use"] = row["use"] ?? 0;
                row["count"] = row["count"] ?? 0;
                row["dropped"] = false;
                yield return row;
            }
        }

        private static string ListTransform(object list) {
            if (list == null)
                return string.Empty;

            var items = (HashSet<string>)list;

            var strings = (from item in items where !string.IsNullOrEmpty(item) select item).ToArray();
            var result = strings.Length > 0 ? string.Join(" | ", strings.OrderBy(s => s)) : string.Empty;
            return result.Equals("System.Object[]") ? string.Empty : result;
        }

        public string FilePath(string outputFolder, Row row) {
            if ((int)row["count"] > 1)
                throw new Exception("You have to write files before you group.");

            var name = Regex.Replace(Regex.Replace(row["name"].ToString(), @"[^\w-]", " ", OPTIONS), @"^\s+|\s+|\s+$", " ", OPTIONS).Trim(' ');
            var path = row["path"].ToString().TrimStart('\\');
            var schema = row["schema"].ToString() == string.Empty ? "dbo" : row["schema"].ToString();

            return Path.Combine(outputFolder, row["server"].ToString(), row["database"].ToString(), schema, path, name) + ".sql";
        }

    }
}
