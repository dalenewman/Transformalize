using System.Collections.Generic;
using System.Linq;
using Transformalize.Rhino.Etl.Core;
using Transformalize.Rhino.Etl.Core.Operations;

namespace Transformalize.Test {

    public class EtlProcessHelper {

        protected List<Row> TestOperation(params IOperation[] operations) {
            return new TestProcess(operations).ExecuteWithResults();
        }

        protected List<Row> TestOperation(IEnumerable<IOperation> operations) {
            return new TestProcess(operations).ExecuteWithResults();
        }

        protected class TestProcess : EtlProcess {
            readonly List<Row> _returnRows = new List<Row>();

            private class ResultsOperation : AbstractOperation {
                readonly List<Row> _rows;

                public ResultsOperation(List<Row> returnRows) {
                    _rows = returnRows;
                }

                public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
                    var r = rows.ToArray();
                    _rows.AddRange(r);
                    return r;
                }
            }

            public TestProcess(params IOperation[] testOperations) {
                this.testOperations = testOperations;
            }

            public TestProcess(IEnumerable<IOperation> testOperations) {
                this.testOperations = testOperations;
            }

            IEnumerable<IOperation> testOperations = null;

            protected override void Initialize() {
                foreach (var testOperation in testOperations)
                    Register(testOperation);

                Register(new ResultsOperation(_returnRows));
            }

            public List<Row> ExecuteWithResults() {
                Execute();
                return _returnRows;
            }

            protected override void PostProcessing() {
                var errors = GetAllErrors().ToArray();
                if (errors.Any())
                    throw errors.First();
            }
        }


    }

}
