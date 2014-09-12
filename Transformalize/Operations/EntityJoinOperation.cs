using System.Collections.Generic;
using System.Linq;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Main;

namespace Transformalize.Operations {
    public class EntityJoinOperation : JoinOperation {
        private readonly Relationship _rel;
        private readonly string[] _fields;

        public EntityJoinOperation(ref Process process, Relationship rel)
            : base(ref process) {
            _rel = rel;
            var rightFields = new HashSet<string>(rel.RightEntity.OutputFields().Aliases());
            rightFields.ExceptWith(rel.LeftEntity.OutputFields().Aliases());
            _fields = rightFields.ToArray();
        }

        protected override Row MergeRows(Row leftRow, Row rightRow) {
            var row = leftRow.Clone();
            foreach (var field in _fields) {
                row[field] = rightRow[field];
            }
            return row;
        }

        protected override void SetupJoinConditions() {
            LeftJoin
                .Left(_rel.Join.Select(j => j.LeftField).Select(f => f.Alias).ToArray())
                .Right(_rel.Join.Select(j => j.RightField).Select(f => f.Alias).ToArray());
        }
    }
}