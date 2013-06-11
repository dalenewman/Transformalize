using System.Collections.Generic;
using System.Configuration;
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
            List<Row> returnRows = new List<Row>();

            private class ResultsOperation : AbstractOperation {
                public ResultsOperation(List<Row> returnRows) {
                    this.returnRows = returnRows;
                }

                List<Row> returnRows = null;

                public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
                    returnRows.AddRange(rows);

                    return rows;
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

                Register(new ResultsOperation(returnRows));
            }

            public List<Row> ExecuteWithResults() {
                Execute();
                return returnRows;
            }

            protected override void PostProcessing() {
                var errors = GetAllErrors().ToArray();
                if (errors.Any())
                    throw errors.First();
            }
        }


    }

}
