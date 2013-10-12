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
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Libs.Rhino.Etl.Pipelines;

namespace Transformalize.Test
{
    public class EtlProcessHelper
    {
        protected List<Row> TestOperation(params IOperation[] operations)
        {
            return new TestProcess(operations).ExecuteWithResults();
        }

        protected List<Row> TestOperation(IEnumerable<IOperation> operations)
        {
            return new TestProcess(operations).ExecuteWithResults();
        }

        protected class TestProcess : EtlProcess
        {
            private readonly List<Row> _returnRows = new List<Row>();
            private readonly IEnumerable<IOperation> _testOperations;

            public TestProcess(params IOperation[] testOperations)
            {
                this.PipelineExecuter = new SingleThreadedNonCachedPipelineExecuter();
                this._testOperations = testOperations;
            }

            public TestProcess(IEnumerable<IOperation> testOperations)
            {
                this._testOperations = testOperations;
            }

            protected override void Initialize()
            {
                foreach (var testOperation in _testOperations)
                    Register(testOperation);

                Register(new ResultsOperation(_returnRows));
            }

            public List<Row> ExecuteWithResults()
            {
                Execute();
                return _returnRows;
            }

            protected override void PostProcessing()
            {
                var errors = GetAllErrors().ToArray();
                if (errors.Any())
                    throw errors.First();
            }

            private class ResultsOperation : AbstractOperation
            {
                private readonly List<Row> _rows;

                public ResultsOperation(List<Row> returnRows)
                {
                    _rows = returnRows;
                }

                public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
                {
                    var r = rows.ToArray();
                    _rows.AddRange(r);
                    return r;
                }
            }
        }
    }
}