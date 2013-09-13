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
    ///     Represent a single operation that can occure during the ETL process
    /// </summary>
    public abstract class AbstractOperation : WithLoggingMixin, IOperation
    {
        private readonly OperationStatistics _statistics = new OperationStatistics();

        /// <summary>
        ///     Gets the pipeline executer.
        /// </summary>
        /// <value>The pipeline executer.</value>
        protected IPipelineExecuter PipelineExecuter { get; private set; }

        /// <summary>
        ///     Gets the name of this instance
        /// </summary>
        /// <value>The name.</value>
        public virtual string Name
        {
            get { return GetType().Name; }
        }

        /// <summary>
        ///     Gets or sets whether we are using a transaction
        /// </summary>
        /// <value>True or false.</value>
        public bool UseTransaction { get; set; }

        /// <summary>
        ///     Gets the statistics for this operation
        /// </summary>
        /// <value>The statistics.</value>
        public OperationStatistics Statistics
        {
            get { return _statistics; }
        }

        /// <summary>
        ///     Occurs when a row is processed.
        /// </summary>
        public virtual event Action<IOperation, Row> OnRowProcessed = delegate { };

        /// <summary>
        ///     Occurs when all the rows has finished processing.
        /// </summary>
        public virtual event Action<IOperation> OnFinishedProcessing = delegate { };

        /// <summary>
        ///     Initializes this instance
        /// </summary>
        /// <param name="pipelineExecuter">The current pipeline executer.</param>
        public virtual void PrepareForExecution(IPipelineExecuter pipelineExecuter)
        {
            PipelineExecuter = pipelineExecuter;
            Statistics.MarkStarted();
        }

        /// <summary>
        ///     Raises the row processed event
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        void IOperation.RaiseRowProcessed(Row dictionary)
        {
            Statistics.MarkRowProcessed();
            OnRowProcessed(this, dictionary);
        }

        /// <summary>
        ///     Raises the finished processing event
        /// </summary>
        void IOperation.RaiseFinishedProcessing()
        {
            Statistics.MarkFinished();
            OnFinishedProcessing(this);
        }

        /// <summary>
        ///     Gets all errors that occured when running this operation
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<Exception> GetAllErrors()
        {
            return Errors;
        }

        /// <summary>
        ///     Executes this operation
        /// </summary>
        /// <param name="rows">The rows.</param>
        /// <returns></returns>
        public abstract IEnumerable<Row> Execute(IEnumerable<Row> rows);

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public virtual void Dispose()
        {
        }
    }
}