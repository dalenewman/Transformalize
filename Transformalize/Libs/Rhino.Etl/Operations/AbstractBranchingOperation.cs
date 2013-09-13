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
    ///     Branch the current pipeline flow into all its inputs
    /// </summary>
    public abstract class AbstractBranchingOperation : AbstractOperation
    {
        /// <summary>
        ///     Creates a new <see cref="AbstractOperation" />
        /// </summary>
        protected AbstractBranchingOperation()
        {
            Operations = new List<IOperation>();
        }

        /// <summary>
        ///     Returns the list of child operations
        /// </summary>
        protected List<IOperation> Operations { get; private set; }

        /// <summary>
        ///     Adds the specified operation to this branching operation
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <returns></returns>
        public AbstractBranchingOperation Add(IOperation operation)
        {
            Operations.Add(operation);
            return this;
        }

        /// <summary>
        ///     Initializes this instance
        /// </summary>
        /// <param name="pipelineExecuter">The current pipeline executer.</param>
        public override void PrepareForExecution(IPipelineExecuter pipelineExecuter)
        {
            base.PrepareForExecution(pipelineExecuter);
            foreach (var operation in Operations)
            {
                operation.PrepareForExecution(pipelineExecuter);
            }
        }

        /// <summary>
        ///     Occurs when    a row is processed.
        /// </summary>
        public override event Action<IOperation, Row> OnRowProcessed
        {
            add
            {
                foreach (var operation in Operations)
                    operation.OnRowProcessed += value;
                base.OnRowProcessed += value;
            }
            remove
            {
                foreach (var operation in Operations)
                    operation.OnRowProcessed -= value;
                base.OnRowProcessed -= value;
            }
        }

        /// <summary>
        ///     Occurs when    all    the    rows has finished processing.
        /// </summary>
        public override event Action<IOperation> OnFinishedProcessing
        {
            add
            {
                foreach (var operation in Operations)
                    operation.OnFinishedProcessing += value;
                base.OnFinishedProcessing += value;
            }
            remove
            {
                foreach (var operation in Operations)
                    operation.OnFinishedProcessing -= value;
                base.OnFinishedProcessing -= value;
            }
        }
    }
}