using System.Linq;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Main;

namespace Transformalize.Operations {
    public class EntityDetectDeletes : JoinOperation {

        private readonly string[] _keys;
        private readonly string _firstKey;

        public EntityDetectDeletes(Process process, Entity entity)
            : base(process) {
            _keys = entity.PrimaryKey.Aliases().ToArray();
            _firstKey = _keys[0];
        }

        protected override Row MergeRows(Row leftRow, Row rightRow) {
            var row = rightRow.Clone();

            if (ThereIsAKeyMatch(leftRow, _firstKey)) {
                row["TflAction"] = EntityAction.None;
                row["TflDeleted"] = false;
            } else {
                var isAlreadyDeleted = rightRow["TflDeleted"] != null && (bool)rightRow["TflDeleted"];
                if (isAlreadyDeleted) {
                    row["TflAction"] = EntityAction.None;
                    row["TflDeleted"] = true;
                } else {
                    row["TflAction"] = EntityAction.Delete;
                    row["TflDeleted"] = true;
                }
            }
            return row;
        }

        private static bool ThereIsAKeyMatch(Row leftRow, string firstKey) {
            return leftRow.ContainsKey(firstKey);
        }

        protected override void SetupJoinConditions() {
            RightJoin.Left(_keys).Right(_keys);
        }
    }
}