using System;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Main;

namespace Transformalize.Libs.Sqloogle.Operations {
    public class CachedObjectStatsJoin : JoinOperation {
        public CachedObjectStatsJoin(Process process) : base(process) {}

        protected override Row MergeRows(Row leftRow, Row rightRow) {
            var row = leftRow.Clone();
            if (rightRow["use"] != null) {
                row["use"] = rightRow["use"];
                row["lastused"] = DateTime.Now;
            }
            return row;
        }

        protected override void SetupJoinConditions() {
            LeftJoin.Left("database", "objectid").Right("database", "objectid");
        }
    }
}