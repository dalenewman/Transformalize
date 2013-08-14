using System.Text;
using Transformalize.Core.Fields_;
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

        public ExpressionTransform(string expression, IParameters parameters, IFields results) : this(null, expression, parameters, results) { }

        public ExpressionTransform(string field, string expression, IParameters parameters, IFields results) : base(parameters, results)
        {
            _field = field ?? "value";
            _expression = new Expression(expression);
            if (!_expression.HasErrors()) return;

            _log.Error("{0} | Expression '{1}' has errors: {2}", Process.Name, expression, _expression.Error);
            System.Environment.Exit(0);
        }

        protected override string Name
        {
            get { return "Expression Transform"; }
        }

        public override void Transform(ref StringBuilder sb)
        {
            _expression.Parameters[_field] = sb.ToString();
            sb.Clear();
            sb.Append(_expression.Evaluate());
        }

        public override void Transform(ref object value)
        {
            _expression.Parameters[_field] = value;
            value = _expression.Evaluate();
        }

        public override void Transform(ref Row row)
        {
            foreach (var pair in Parameters)
            {
                _expression.Parameters[pair.Value.Name] =  pair.Value.Value ?? row[pair.Key];
            }
            row[FirstResult.Key] = _expression.Evaluate();
        }

    }
}