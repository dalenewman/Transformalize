#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;
using System.Collections.Generic;
using Transformalize.Main;

namespace Transformalize.Libs.Rhino.Etl.Operations
{
    /// <summary>
    ///     A partial process that can take part in another process
    /// </summary>
    public class PartialProcessOperation : EtlProcessBase<PartialProcessOperation>, IOperation
    {
        private readonly OperationStatistics statistics = new OperationStatistics();
        private IPipelineExecuter pipelineExeuter;

        public PartialProcessOperation(Process process) : base(process) {}

        /// <summary>
        ///     Occurs when all the rows has finished processing.
        /// </summary>
        public event Action<IOperation> OnFinishedProcessing
        {
            add
            {
                foreach (var operation in Operations)
                {
                    operation.OnFinishedProcessing += value;
                }
            }
            remove
            {
                foreach (var operation in Operations)
                {
                    operation.OnFinishedProcessing -= value;
                }
            }
        }

        /// <summary>
        ///     Initializes the current instance
        /// </summary>
        /// <param name="pipelineExecuter">The current pipeline executer.</param>
        public void PrepareForExecution(IPipelineExecuter pipelineExecuter)
        {
            pipelineExeuter = pipelineExecuter;
            foreach (var operation in Operations)
            {
                operation.PrepareForExecution(pipelineExecuter);
            }
            Statistics.MarkStarted();
        }

        public long LogRows { get; set; }
        public string ProcessName { get; set; }
        public string EntityName { get; set; }

        /// <summary>
        ///     Gets the statistics for this operation
        /// </summary>
        /// <value>The statistics.</value>
        public OperationStatistics Statistics
        {
            get { return statistics; }
        }

        /// <summary>
        ///     Occurs when a row is processed.
        /// </summary>
        public event Action<IOperation, Row> OnRowProcessed
        {
            add
            {
                foreach (var operation in Operations)
                {
                    operation.OnRowProcessed += value;
                }
            }
            remove
            {
                foreach (var operation in Operations)
                {
                    operation.OnRowProcessed -= value;
                }
            }
        }

        /// <summary>
        ///     Executes this operation
        /// </summary>
        /// <param name="rows">The rows.</param>
        /// <returns></returns>
        public IEnumerable<Row> Execute(IEnumerable<Row> rows)
        {
            MergeLastOperationsToOperations();
            return pipelineExeuter.PipelineToEnumerable(Operations, rows, enumerable => enumerable);
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            foreach (var operation in Operations)
            {
                operation.Dispose();
            }
        }

        /// <summary>
        ///     Raises the row processed event
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        void IOperation.RaiseRowProcessed(Row dictionary)
        {
            Statistics.MarkRowProcessed();
            // we don't have a real event here, so we ignore it
            // it will be handled by the children at any rate
        }

        /// <summary>
        ///     Raises the finished processing event
        /// </summary>
        void IOperation.RaiseFinishedProcessing()
        {
            Statistics.MarkFinished();
            // we don't have a real event here, so we ignore it
            // it will be handled by the children at any rate
        }

        /// <summary>
        ///     Gets all errors that occured when running this operation
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Exception> GetAllErrors()
        {
            foreach (var operation in Operations)
            {
                foreach (var error in operation.GetAllErrors())
                {
                    yield return error;
                }
            }
        }
    }
}