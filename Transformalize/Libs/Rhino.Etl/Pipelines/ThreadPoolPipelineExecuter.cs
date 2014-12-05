#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;
using System.Collections.Generic;
using System.Threading;
using Transformalize.Extensions;
using Transformalize.Libs.Rhino.Etl.Enumerables;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Main;

namespace Transformalize.Libs.Rhino.Etl.Pipelines {
    /// <summary>
    ///     Execute all the actions concurrently, in the thread pool
    /// </summary>
    public class ThreadPoolPipelineExecuter : AbstractPipelineExecuter {
        /// <summary>
        ///     Add a decorator to the enumerable for additional processing
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="enumerator">The enumerator.</param>
        protected override IEnumerable<Row> DecorateEnumerableForExecution(IOperation operation, IEnumerable<Row> enumerator) {
            var threadedEnumerator = new ThreadSafeEnumerator<Row>();
            ThreadPool.QueueUserWorkItem(delegate {
                try {
                    foreach (Row t in new EventRaisingEnumerator(operation, enumerator)) {
                        threadedEnumerator.AddItem(t);
                    }
                } catch (Exception e) {
                    foreach (var inner in e.FlattenHierarchy()) {
                        Error("Failed to execute {0}. {1}", operation.Name, inner.Message);
                        Debug(inner.StackTrace);
                    }
                    threadedEnumerator.MarkAsFinished();
#if DEBUG
                    throw new TransformalizeException(string.Empty, string.Empty, e.Message);
#endif
                    
                } finally {
                    threadedEnumerator.MarkAsFinished();
                }
            });
            return threadedEnumerator;
        }
    }
}