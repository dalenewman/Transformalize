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
using Transformalize.Libs.Rhino.Etl.Operations;

namespace Transformalize.Libs.Rhino.Etl
{
    /// <summary>
    ///     Interface that abastract the actual execution of the pipeline
    /// </summary>
    public interface IPipelineExecuter
    {
        /// <summary>
        ///     Gets a value indicating whether this instance has errors.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance has errors; otherwise, <c>false</c>.
        /// </value>
        bool HasErrors { get; }

        /// <summary>
        ///     Executes the specified pipeline.
        /// </summary>
        /// <param name="pipelineName">The name.</param>
        /// <param name="pipeline">The pipeline.</param>
        /// <param name="translateRows">Translate the rows into another representation</param>
        void Execute(string pipelineName,
                     ICollection<IOperation> pipeline,
                     Func<IEnumerable<Row>, IEnumerable<Row>> translateRows);

        /// <summary>
        ///     Transform the pipeline to an enumerable
        /// </summary>
        /// <param name="pipeline">The pipeline.</param>
        /// <param name="rows">The rows.</param>
        /// <param name="translateEnumerable">Translate the rows from one representation to another</param>
        /// <returns></returns>
        IEnumerable<Row> PipelineToEnumerable(
            ICollection<IOperation> pipeline,
            IEnumerable<Row> rows,
            Func<IEnumerable<Row>, IEnumerable<Row>> translateEnumerable);

        /// <summary>
        ///     Gets all errors that occured under this executer
        /// </summary>
        /// <returns></returns>
        IEnumerable<Exception> GetAllErrors();
    }
}