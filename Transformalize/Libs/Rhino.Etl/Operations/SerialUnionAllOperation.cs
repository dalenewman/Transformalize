#region License
// /*
// See license included in this library folder.
// */
#endregion

using System.Collections.Generic;
using System.Linq;
using Transformalize.Main;

namespace Transformalize.Libs.Rhino.Etl.Operations {
    public class SerialUnionAllOperation : AbstractOperation {
        private readonly List<IOperation> _operations = new List<IOperation>();

        public SerialUnionAllOperation(Entity entity, string operationColumn = "operation") {
            OperationColumn = operationColumn;
            ProcessName = entity.ProcessName;
            EntityName = entity.Name;
        }

        public string OperationColumn { get; set; }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            if (_operations.Count > 0) {
                foreach (var row in _operations.SelectMany(operation => operation.Execute(null))) {
                    yield return row;
                }
            } else {
                foreach (var innerRow in rows.SelectMany(row => ((IOperation)row[OperationColumn]).Execute(null))) {
                    yield return innerRow;
                }
            }
        }

        public SerialUnionAllOperation Add(params IOperation[] operation) {
            _operations.AddRange(operation);
            return this;
        }

        public override void PrepareForExecution(IPipelineExecuter pipelineExecuter) {
            foreach (var operation in _operations) {
                operation.PrepareForExecution(pipelineExecuter);
            }
        }
    }
}