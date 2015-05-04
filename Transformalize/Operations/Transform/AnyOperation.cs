using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Transformalize.Libs.EnterpriseLibrary.Validation.Validators;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Main;
using Transformalize.Main.Parameters;

namespace Transformalize.Operations.Transform {
    public class AnyOperation : ShouldRunOperation {
        private readonly ComparisonOperator _op;
        private readonly IParameters _parameters;
        private readonly bool _negated;

        private readonly KeyValuePair<string, IParameter> _left;
        private readonly bool _leftHasValue;
        private readonly object _leftValue;
        private readonly string[] _parameterKeys;

        private readonly Dictionary<string, KeyValuePair<string, IParameter>> _builtIns = new Dictionary<string, KeyValuePair<string, IParameter>> {
            {"true", new KeyValuePair<string, IParameter>("true", new Parameter("true", true) { SimpleType = "boolean"} )},
            {"false", new KeyValuePair<string, IParameter>("false", new Parameter("false", false) { SimpleType = "boolean"} )},
            {string.Empty, new KeyValuePair<string, IParameter>(string.Empty, new Parameter(string.Empty, string.Empty))}
        };

        public AnyOperation(IParameter leftParameter, string outKey, ComparisonOperator op, IParameters parameters, bool negated)
            : base(string.Empty, outKey) {
            _op = op;
            _parameters = parameters;
            _negated = negated;
            _left = _builtIns.ContainsKey(leftParameter.Name.ToLower()) ? _builtIns[leftParameter.Name.ToLower()] : new KeyValuePair<string, IParameter>(leftParameter.Name, leftParameter);
            _leftHasValue = _left.Value.HasValue();
            _leftValue = _left.Value.Value;
            _parameterKeys = parameters.Keys.ToArray();

            Name = string.Format("AnyOperation ({0})", outKey);

            if (Common.CompareMap.ContainsKey(op))
                return;

            throw new TransformalizeException(Logger, EntityName, "Operator {0} is invalid.  Try equal, notequal, greaterthan, greaterthanequal, greaterthan, or greaterthanequal.");

        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                if (ShouldRun(row)) {
                    var leftValue = _leftHasValue ? _leftValue : row[_left.Key];
                    var r = row;
                    var answer = _parameterKeys.Select(k => r.ContainsKey(k) ? r[k] : _parameters[k].Value).Any(v => Common.CompareMap[_op](v, leftValue));
                    row[OutKey] = _negated ? !answer : answer;
                } else {
                    Interlocked.Increment(ref SkipCount);
                }
                yield return row;
            }
        }
    }
}