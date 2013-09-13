using System;
using System.Collections.Generic;
using System.Linq;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;

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
            private readonly IEnumerable<IOperation> testOperations;

            public TestProcess(params IOperation[] testOperations) : base("Test")
            {
                this.testOperations = testOperations;
            }

            public TestProcess(IEnumerable<IOperation> testOperations) : base("Test")
            {
                this.testOperations = testOperations;
            }

            protected override void Initialize()
            {
                foreach (IOperation testOperation in testOperations)
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
                Exception[] errors = GetAllErrors().ToArray();
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
                    Row[] r = rows.ToArray();
                    _rows.AddRange(r);
                    return r;
                }
            }
        }
    }
}