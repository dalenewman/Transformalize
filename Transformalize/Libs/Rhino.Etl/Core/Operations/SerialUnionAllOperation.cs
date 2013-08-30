using System.Collections.Generic;
using System.Linq;
using Transformalize.Libs.NLog;
using Transformalize.Operations;

namespace Transformalize.Libs.Rhino.Etl.Core.Operations
{
    public class SerialUnionAllOperation : AbstractOperation
    {

        private readonly List<IOperation> _operations = new List<IOperation>();
        public string OperationColumn { get; set; }
        private readonly Logger _log = LogManager.GetCurrentClassLogger();

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
            foreach (var innerRow in rows.Select(row => (Transformalize.Operations.EntityDataExtract)row[OperationColumn]).SelectMany(operation => operation.Execute(null)))
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