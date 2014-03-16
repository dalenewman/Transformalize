#region License
// /*
// See license included in this library folder.
// */
#endregion

using System.Collections.Generic;
using System.Linq;
using Transformalize.Operations;

namespace Transformalize.Libs.Rhino.Etl.Operations {
    public class SerialUnionAllOperation : AbstractOperation {
        private readonly List<IOperation> _operations = new List<IOperation>();

        public SerialUnionAllOperation(string operationColumn = "operation") {
            OperationColumn = operationColumn;
        }

        public SerialUnionAllOperation(IEnumerable<IOperation> operations) {
            _operations.AddRange(operations);
        }

        public SerialUnionAllOperation(params IOperation[] operations) {
            _operations.AddRange(operations);
        }

        public string OperationColumn { get; set; }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            if (_operations.Count > 0) {
                foreach (var row in _operations.SelectMany(operation => operation.Execute(null))) {
                    yield return row;
                }
            } else {
                foreach (var row in rows) {
                    var operation = (EntityDataExtract)row[OperationColumn];
                    foreach (var innerRow in operation.Execute(null)) {
                        yield return innerRow;
                    }
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