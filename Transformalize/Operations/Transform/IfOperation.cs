using System;
using System.Collections.Generic;
using System.Linq;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Main;

namespace Transformalize.Operations.Transform {
    public class IfOperation : AbstractOperation {

        private const StringComparison IC = StringComparison.OrdinalIgnoreCase;
        private readonly string _op;
        private readonly string _outKey;

        private readonly KeyValuePair<string, IParameter> _left;
        private readonly KeyValuePair<string, IParameter> _right;
        private readonly KeyValuePair<string, IParameter> _then;
        private readonly KeyValuePair<string, IParameter> _else;

        private readonly bool _leftHasValue;
        private readonly bool _rightHasValue;
        private readonly bool _thenHasValue;
        private readonly bool _elseHasValue;

        private readonly object _leftValue;
        private readonly object _rightValue;
        private readonly object _thenValue;
        private readonly object _elseValue;

        private readonly Dictionary<string, KeyValuePair<string, IParameter>> _builtIns = new Dictionary<string, KeyValuePair<string, IParameter>> {
            {"true", new KeyValuePair<string, IParameter>("true", new Parameter("true", true) { SimpleType = "boolean"} )},
            {"false", new KeyValuePair<string, IParameter>("false", new Parameter("false", false) { SimpleType = "boolean"} )},
            {string.Empty, new KeyValuePair<string, IParameter>(string.Empty, new Parameter(string.Empty, string.Empty))}
        };

        private readonly Dictionary<string, Func<object, object, bool>> _compare = new Dictionary<string, Func<object, object, bool>>() {
            {"=", ((x, y) => x.Equals(y))},
            {"!=", ((x, y) => !x.Equals(y))},
            {">", ((x, y) => ((IComparable) x).CompareTo(y) > 0)},
            {">=", ((x, y) => x.Equals(y) || ((IComparable)x).CompareTo(y) > 0)},
            {"<", ((x, y) => ((IComparable)x).CompareTo(y) < 0)},
            {"<=", ((x, y) => x.Equals(y) || ((IComparable)x).CompareTo(y) < 0)}
        };

        public IfOperation(string leftKey, string op, string rightKey, string thenKey, string elseKey, IParameters parameters, string outKey, string outType) {

            _op = op;
            _outKey = outKey;

            var param = parameters.ToEnumerable().ToArray();

            _left = _builtIns.ContainsKey(leftKey.ToLower()) ? _builtIns[leftKey.ToLower()] : GetPart(param, leftKey);
            _right = _builtIns.ContainsKey(rightKey.ToLower()) ? _builtIns[rightKey.ToLower()] : GetPart(param, rightKey);
            _then = _builtIns.ContainsKey(thenKey.ToLower()) ? _builtIns[thenKey.ToLower()] : GetPart(param, thenKey);
            _else = _builtIns.ContainsKey(elseKey.ToLower()) ? _builtIns[elseKey.ToLower()] : GetPart(param, elseKey);

            _leftHasValue = _left.Value.HasValue();
            _rightHasValue = _right.Value.HasValue();
            _thenHasValue = _then.Value.HasValue();
            _elseHasValue = _else.Value.HasValue();

            _leftValue = _leftHasValue ? Common.ObjectConversionMap[_left.Value.SimpleType](_left.Value.Value) : null;
            _rightValue = _rightHasValue ? Common.ObjectConversionMap[_right.Value.SimpleType](_right.Value.Value) : null;
            _thenValue = _thenHasValue ? Common.ObjectConversionMap[outType](_then.Value.Value) : null;
            _elseValue = _elseHasValue ? Common.ObjectConversionMap[outType](_else.Value.Value) : null;

            if (_compare.ContainsKey(op))
                return;

            Error("Operator {0} is invalid.  Try =, !=, >, >=, <, or <=.");
            Environment.Exit(1);
        }

        private static KeyValuePair<string, IParameter> GetPart(KeyValuePair<string, IParameter>[] parameters, string parameter) {
            if (!parameters.Any(p => p.Value.Name.Equals(parameter, IC))) {
                //Error("An if transform is missing it's {0} parameter.", parameter);
                //Environment.Exit(1);
                return new KeyValuePair<string, IParameter>(parameter, new Parameter(parameter, parameter));
            }

            return parameters.First(p => p.Value.Name.Equals(parameter, IC));
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {

                var leftValue = _leftHasValue ? _leftValue : row[_left.Key];
                var rightValue = _rightHasValue ? _rightValue : row[_right.Key];

                if (_compare[_op](leftValue, rightValue)) {
                    row[_outKey] = _thenHasValue ? _thenValue : row[_then.Key];
                } else {
                    row[_outKey] = _elseHasValue ? _elseValue : row[_else.Key];
                }
                yield return row;
            }
        }
    }
}