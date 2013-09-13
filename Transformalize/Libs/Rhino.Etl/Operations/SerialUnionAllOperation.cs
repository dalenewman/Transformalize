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

using System.Collections.Generic;
using System.Linq;
using Transformalize.Libs.NLog;
using Transformalize.Operations;

namespace Transformalize.Libs.Rhino.Etl.Operations
{
    public class SerialUnionAllOperation : AbstractOperation
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        private readonly List<IOperation> _operations = new List<IOperation>();

        public SerialUnionAllOperation(string operationColumn = "operation")
        {
            OperationColumn = operationColumn;
        }

        public SerialUnionAllOperation(IEnumerable<IOperation> operations)
        {
            _operations.AddRange(operations);
        }

        public SerialUnionAllOperation(params IOperation[] operations)
        {
            _operations.AddRange(operations);
        }

        public string OperationColumn { get; set; }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
        {
            if (_operations.Count > 0)
            {
                foreach (var row in _operations.SelectMany(operation => operation.Execute(null)))
                {
                    yield return row;
                }
            }

            //todo: try traditional data read
            foreach (var innerRow in rows.Select(row => (EntityDataExtract) row[OperationColumn]).SelectMany(operation => operation.Execute(null)))
            {
                yield return innerRow;
            }
        }

        public SerialUnionAllOperation Add(params IOperation[] operation)
        {
            _operations.AddRange(operation);
            return this;
        }

        public override void PrepareForExecution(IPipelineExecuter pipelineExecuter)
        {
            foreach (var operation in _operations)
            {
                operation.PrepareForExecution(pipelineExecuter);
            }
        }
    }
}