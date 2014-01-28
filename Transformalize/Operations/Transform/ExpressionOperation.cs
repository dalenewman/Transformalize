using System;
using System.Collections.Generic;
using Transformalize.Libs.NCalc;
using Transformalize.Libs.NLog;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Main;

namespace Transformalize.Operations.Transform {

    public class ExpressionOperation : TflOperation {
        private readonly Logger _log = LogManager.GetLogger(string.Empty);
        private readonly Expression _expression;
        private readonly IParameters _parameters;

        public ExpressionOperation(string outKey, string expression, IParameters parameters)
            : base(string.Empty, outKey) {
            _parameters = parameters;
            _expression = new Expression(expression);
            if (!_expression.HasErrors())
                return;

            _log.Error("Expression '{0}' has errors: {1}", expression, _expression.Error);
            Environment.Exit(0);
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {

            foreach (var row in rows) {
                if (ShouldRun(row)) {
                    _expression.Parameters[OutKey] = row[OutKey];
                    foreach (var pair in _parameters) {
                        _expression.Parameters[pair.Value.Name] = pair.Value.Value ?? row[pair.Key];
                    }

                    row[OutKey] = _expression.Evaluate();
                }

                yield return row;
            }
        }
    }
}