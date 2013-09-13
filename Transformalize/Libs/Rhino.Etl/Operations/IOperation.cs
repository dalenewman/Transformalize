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
    ///     A single operation in an etl process
    /// </summary>
    public interface IOperation : IDisposable
    {
        /// <summary>
        ///     Gets the name.
        /// </summary>
        /// <value>The name.</value>
        string Name { get; }

        /// <summary>
        ///     Sets the transaction.
        /// </summary>
        /// <value>True or false.</value>
        bool UseTransaction { get; set; }

        /// <summary>
        ///     Gets the statistics for this operation
        /// </summary>
        /// <value>The statistics.</value>
        OperationStatistics Statistics { get; }

        /// <summary>
        ///     Occurs when a row is processed.
        /// </summary>
        event Action<IOperation, Row> OnRowProcessed;

        /// <summary>
        ///     Occurs when all the rows has finished processing.
        /// </summary>
        event Action<IOperation> OnFinishedProcessing;

        /// <summary>
        ///     Initializes the current instance
        /// </summary>
        /// <param name="pipelineExecuter">The current pipeline executer.</param>
        void PrepareForExecution(IPipelineExecuter pipelineExecuter);

        /// <summary>
        ///     Executes this operation
        /// </summary>
        /// <param name="rows">The rows.</param>
        IEnumerable<Row> Execute(IEnumerable<Row> rows);

        /// <summary>
        ///     Raises the row processed event
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        void RaiseRowProcessed(Row dictionary);

        /// <summary>
        ///     Raises the finished processing event
        /// </summary>
        void RaiseFinishedProcessing();

        /// <summary>
        ///     Gets all errors that occured when running this operation
        /// </summary>
        /// <returns></returns>
        IEnumerable<Exception> GetAllErrors();
    }
}