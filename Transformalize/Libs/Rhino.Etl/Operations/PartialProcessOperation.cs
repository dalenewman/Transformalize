#region License

// /*
// Transformalize - Replicate, Transform, and Denormalize Your Data...
// Copyright (C) 2013 Dale Newman
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// */

#endregion

using System;
using System.Collections.Generic;

namespace Transformalize.Libs.Rhino.Etl.Operations
{
    /// <summary>
    ///     A partial process that can take part in another process
    /// </summary>
    public class PartialProcessOperation : EtlProcessBase<PartialProcessOperation>, IOperation
    {
        private readonly OperationStatistics statistics = new OperationStatistics();
        private IPipelineExecuter pipelineExeuter;

        /// <summary>
        ///     Occurs when all the rows has finished processing.
        /// </summary>
        public event Action<IOperation> OnFinishedProcessing
        {
            add
            {
                foreach (var operation in operations)
                {
                    operation.OnFinishedProcessing += value;
                }
            }
            remove
            {
                foreach (var operation in operations)
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
            foreach (var    operation in operations)
            {
                operation.PrepareForExecution(pipelineExecuter);
            }
            Statistics.MarkStarted();
        }

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
                foreach (var operation in operations)
                {
                    operation.OnRowProcessed += value;
                }
            }
            remove
            {
                foreach (var operation in operations)
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
            return pipelineExeuter.PipelineToEnumerable(operations, rows, enumerable => enumerable);
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            foreach (var operation in operations)
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
            foreach (var operation in operations)
            {
                foreach (var error in operation.GetAllErrors())
                {
                    yield return error;
                }
            }
        }
    }
}