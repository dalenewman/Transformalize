using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Transformalize.Rhino.Etl.Core.Operations {

    /// <summary>
    /// Combines rows from all operations.
    /// </summary>
    public class ParallelUnionAllOperation : AbstractOperation {

        private readonly List<IOperation> _operations = new List<IOperation>();

        public ParallelUnionAllOperation() { }

        public ParallelUnionAllOperation(IEnumerable<IOperation> ops) {
            _operations.AddRange(ops);
        }

        public ParallelUnionAllOperation(params IOperation[] ops) {
            _operations.AddRange(ops);
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            var blockingCollection = new BlockingCollection<Row>();
            var count = _operations.Count;
            if (count == 0) {
                yield break;
            }

            Debug("Creating tasks for {0} operations.", count);

            var tasks = _operations.Select(currentOp =>
            Task.Factory.StartNew(() => {
                try {
                    foreach (var row in currentOp.Execute(null)) {
                        blockingCollection.Add(row);
                    }
                }
                finally {
                    if (Interlocked.Decrement(ref count) == 0) {
                        blockingCollection.CompleteAdding();
                    }
                }
            })).ToArray();

            foreach (var row in blockingCollection.GetConsumingEnumerable()) {
                yield return row;
            }
            Task.WaitAll(tasks); // raise any exception that were raised during execution
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

        /// <summary>
        /// Add operation parameters
        /// </summary>
        /// <param name="ops">operations delimited by commas</param>
        /// <returns></returns>
        public ParallelUnionAllOperation Add(params IOperation[] ops) {
            _operations.AddRange(ops);
            return this;
        }

        /// <summary>
        /// Add operations
        /// </summary>
        /// <param name="ops">an enumerable of operations</param>
        /// <returns></returns>
        public ParallelUnionAllOperation Add(IEnumerable<IOperation> ops) {
            _operations.AddRange(ops);
            return this;
        }

    }
}
