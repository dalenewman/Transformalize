using System.Collections.Generic;

namespace Transformalize.Rhino.Etl.Core.Operations {
    public class SerialUnionAllOperation : AbstractOperation {

        private readonly List<IOperation> _operations = new List<IOperation>();

        public SerialUnionAllOperation() { }

        public SerialUnionAllOperation(IEnumerable<IOperation> ops) {
            _operations.AddRange(ops);
        }

        public SerialUnionAllOperation(params IOperation[] ops) {
            _operations.AddRange(ops);
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var operation in _operations)
                foreach (var row in operation.Execute(null))
                    yield return row;
        }

        public SerialUnionAllOperation Add(params IOperation[] operation) {
            _operations.AddRange(operation);
            return this;
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