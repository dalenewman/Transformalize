using System.Collections.Generic;
using System.Linq;

namespace Transformalize.Rhino.Etl.Core.Operations {
    public class ConventionSerialUnionAllOperation : AbstractOperation {

        public string OperationColumn { get; set; }
        private readonly List<IOperation> _operations = new List<IOperation>();

        public ConventionSerialUnionAllOperation(string operationColumn = "operation") {
            OperationColumn = operationColumn;
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            return rows.Select(row => row[OperationColumn]).Cast<IOperation>().SelectMany(operation => operation.Execute(null));
        }

        /// <summary>
        /// Initializes this instance
        /// </summary>
        /// <param name="pipelineExecuter">The current pipeline executer.</param>
        public override void PrepareForExecution(IPipelineExecuter pipelineExecuter) {
            foreach (var operation in _operations) {
                operation.PrepareForExecution(pipelineExecuter);
            }
        }

    }
}