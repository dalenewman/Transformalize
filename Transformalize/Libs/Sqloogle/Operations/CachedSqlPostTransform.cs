using System;
using System.Collections.Generic;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;

namespace Transformalize.Libs.Sqloogle.Operations {
    public class CachedSqlPostTransform : AbstractOperation {

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {

            foreach (var row in rows) {
                row["schema"] = string.Empty;
                row["name"] = string.Empty;
                row["path"] = string.Empty;
                row["created"] = DateTime.Now;
                row["modified"] = DateTime.Now;
                row["lastused"] = DateTime.Now;
                yield return row;
            }

        }
    }
}
