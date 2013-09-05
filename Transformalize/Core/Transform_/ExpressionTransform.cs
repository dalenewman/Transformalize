using System.Text;
using Transformalize.Core.Parameters_;
using Transformalize.Core.Process_;
using Transformalize.Libs.NCalc;
using Transformalize.Libs.NLog;
using Transformalize.Libs.Rhino.Etl.Core;

namespace Transformalize.Core.Transform_
{
    public class ExpressionTransform : AbstractTransform
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        private readonly string _field;
        private readonly Expression _expression;

        public ExpressionTransform(string expression, IParameters parameters) : this(null, expression, parameters) { }

        public ExpressionTransform(string field, string expression, IParameters parameters)
            : base(parameters)
        {
            Name = "Expression";
            _field = field ?? "value";
            _expression = new Expression(expression);
            if (!_expression.HasErrors()) return;

            _log.Error("Expression '{0}' has errors: {1}", expression, _expression.Error);
            System.Environment.Exit(0);

        }

        public override void Transform(ref StringBuilder sb)
        {
            _expression.Parameters[_field] = sb.ToString();
            sb.Clear();
            sb.Append(_expression.Evaluate());
        }

        public override object Transform(object value)
        {
            _expression.Parameters[_field] = value;
            return _expression.Evaluate();
        }

        public override void Transform(ref Row row, string resultKey)
        {
            foreach (var pair in Parameters)
            {
                _expression.Parameters[pair.Value.Name] = pair.Value.Value ?? row[pair.Key];
            }
            row[resultKey] = _expression.Evaluate();
        }

    }
}