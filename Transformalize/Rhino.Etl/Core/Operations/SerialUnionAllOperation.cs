using System.Collections.Generic;
using System.Linq;

namespace Transformalize.Rhino.Etl.Core.Operations {
    public class SerialUnionAllOperation : AbstractOperation {

        private readonly List<IOperation> _operations = new List<IOperation>();
        public string OperationColumn { get; set; }

        public SerialUnionAllOperation(string operationColumn = "operation") {
            OperationColumn = operationColumn;
        }

        public SerialUnionAllOperation(IEnumerable<IOperation> operations) {
            _operations.AddRange(operations);
        }

        public SerialUnionAllOperation(params IOperation[] operations) {
            _operations.AddRange(operations);
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            if (_operations.Count > 0)
                return _operations.SelectMany(operation => operation.Execute(null));

            return rows.Select(row => row[OperationColumn]).Cast<IOperation>().SelectMany(operation => operation.Execute(null));
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