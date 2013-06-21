using System;
using System.Collections.Generic;
using System.Threading;
using Transformalize.Rhino.Etl.Core.Enumerables;
using Transformalize.Rhino.Etl.Core.Operations;

namespace Transformalize.Rhino.Etl.Core.Pipelines
{
    /// <summary>
    /// Execute all the actions concurrently, in the thread pool
    /// </summary>
    public class ThreadPoolPipelineExecuter : AbstractPipelineExecuter
    {
       /// <summary>
        /// Add a decorator to the enumerable for additional processing
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="enumerator">The enumerator.</param>
        protected override IEnumerable<Row> DecorateEnumerableForExecution(IOperation operation, IEnumerable<Row> enumerator)
        {
            var threadedEnumerator = new ThreadSafeEnumerator<Row>();
            ThreadPool.QueueUserWorkItem(delegate
            {
                try
                {
                    foreach (Row t in new EventRaisingEnumerator(operation, enumerator))
                    {
                        threadedEnumerator.AddItem(t);
                    }
                }
                catch (Exception e)
                {
                    Error(e, "Failed to execute operation {0}", operation);
                    threadedEnumerator.MarkAsFinished();
#if DEBUG
                    throw e;
#endif
                }
                finally
                {
                    threadedEnumerator.MarkAsFinished();
                }
            });
            return threadedEnumerator;
        }
    }
}