using System.Linq;
using Transformalize.Model;
using Transformalize.Rhino.Etl.Core;
using Transformalize.Rhino.Etl.Core.Operations;

namespace Transformalize.Operations
{
    public class EntityJoinAction : JoinOperation {
        private readonly string[] _keys;
        private readonly string _firstKey;
        private readonly int _tflBatchId;

        public EntityJoinAction(Entity entity) {
            _keys = entity.PrimaryKey.Select(e => e.Value.Alias).ToArray();
            _firstKey = _keys[0];
            _tflBatchId = entity.TflBatchId;
        }

        protected override Row MergeRows(Row leftRow, Row rightRow) {

            if (rightRow.ContainsKey(_firstKey)) {
                leftRow["a"] = EntityAction.Update;
                leftRow["TflKey"] = rightRow["TflKey"];
            } else {
                leftRow["a"] = EntityAction.Insert;
                leftRow["TflBatchId"] = _tflBatchId;
            }

            return leftRow;
        }

        protected override void SetupJoinConditions() {
            LeftJoin.Left(_keys).Right(_keys);
        }
    }
}