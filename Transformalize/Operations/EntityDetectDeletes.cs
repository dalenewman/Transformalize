using System.Linq;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Main;

namespace Transformalize.Operations
{
    public class EntityDetectDeletes : JoinOperation {

        private readonly string[] _keys;
        private readonly string _firstKey;

        public EntityDetectDeletes(Entity entity) {
            _keys = entity.PrimaryKey.Keys.ToArray();
            _firstKey = _keys[0];
        }

        protected override Row MergeRows(Row leftRow, Row rightRow) {
            var row = rightRow.Clone();
            if (leftRow.ContainsKey(_firstKey)) {
                row["TflAction"] = EntityAction.None;
            } else {
                row["TflAction"] = EntityAction.Delete;
            }
            return row;
        }

        protected override void SetupJoinConditions() {
            RightJoin.Left(_keys).Right(_keys);
        }
    }
}