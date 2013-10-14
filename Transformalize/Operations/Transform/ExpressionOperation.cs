using System;
using System.Collections.Generic;
using Transformalize.Libs.NCalc;
using Transformalize.Libs.NLog;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Main;

namespace Transformalize.Operations.Transform {
    public class ExpressionOperation : AbstractOperation {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        private readonly Expression _expression;
        private readonly string _outKey;
        private readonly IParameters _parameters;

        public ExpressionOperation(string outKey, string expression, IParameters parameters) {
            _outKey = outKey;
            _parameters = parameters;
            _expression = new Expression(expression);
            if (!_expression.HasErrors())
                return;

            _log.Error("Expression '{0}' has errors: {1}", expression, _expression.Error);
            Environment.Exit(0);
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {

            foreach (var row in rows) {
                _expression.Parameters[_outKey] = row[_outKey];
                foreach (var pair in _parameters) {
                    _expression.Parameters[pair.Value.Name] = pair.Value.Value ?? row[pair.Key];
                }

                row[_outKey] = _expression.Evaluate();

                yield return row;
            }
        }
    }
}