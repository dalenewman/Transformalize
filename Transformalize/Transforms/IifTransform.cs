#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2019 Dale Newman
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//   
//       http://www.apache.org/licenses/LICENSE-2.0
//   
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Transformalize.Contracts;

namespace Transformalize.Transforms
{

    // ReSharper disable once InconsistentNaming
    public class IIfTransform : BaseTransform
    {

        private class ExpressionEvaluator
        {

            private readonly Regex _leftSide = new Regex("^[^!=></^/$/*]+");
            private readonly Regex _rightSide = new Regex("[^!=></^/$/*]+$");
            private readonly Regex _operator = new Regex("(?<!^)[!=<>/^/$/*]+(?!$)");

            private readonly Func<IRow, int> _compare;
            private readonly Func<int, IField, IField, IField> _choose;

            private bool LeftIsField { get; set; }
            private bool RightIsField { get; set; }
            private IField LeftField { get; set; }
            private IField RightField { get; set; }
            private string RightFieldInput { get; set; }
            private string LeftFieldInput { get; set; }
            private IField TrueField { get; }
            private IField FalseField { get; }

            public object Evaluate(IRow row)
            {
                return row[_choose(_compare(row), TrueField, FalseField)];
            }

            public ExpressionEvaluator(IContext context)
            {
                ProcessLeftSide(context);
                ProcessRightSide(context);

                var operatorMatch = _operator.Match(context.Operation.Expression);
                var @operator = operatorMatch.Success ? operatorMatch.Value.Trim() : string.Empty;

                switch (@operator)
                {
                    case "=":
                    case "==":
                        _choose = (i, trueField, falseField) => i == 0 ? trueField : falseField;
                        break;
                    case "!=":
                        _choose = (i, tf, ff) => i == 0 ? ff : tf;
                        break;
                    case ">":
                        _choose = (i, tf, ff) => i > 0 ? tf : ff;
                        break;
                    case "<":
                        _choose = (i, tf, ff) => i < 0 ? tf : ff;
                        break;
                    case ">=":
                        _choose = (i, tf, ff) => i >= 0 ? tf : ff;
                        break;
                    case "<=":
                        _choose = (i, tf, ff) => i <= 0 ? tf : ff;
                        break;
                    case "^=": // starts with
                        _choose = (i, tf, ff) => i > 0 ? tf : ff;
                        break;
                    case "$=": // ends with
                        _choose = (i, tf, ff) => i > 0 ? tf : ff;
                        break;
                    case "*=": // contains
                        _choose = (i, tf, ff) => i > 0 ? tf : ff;
                        break;
                    default:
                        context.Warn($"{@operator} is an invalid operator");
                        _choose = (i, tf, ff) => ff;
                        break;
                }


                if (LeftIsField && RightIsField && LeftField.Type == RightField.Type)
                {
                    _compare = (row) =>
                    {
                        switch (@operator)
                        {
                            case "^=":
                                if (LeftField.Type == "string" && RightField.Type == "string")
                                {
                                    return ((string)row[LeftField]).StartsWith((string)row[RightField]) ? 1 : 0;
                                }
                                context.Warn($"The expression {context.Operation.Expression} has a startsWith (^=) which may only operator on strings.  Your left field is {LeftField.Type}, and your right field is {RightField.Type}.");
                                return -1;
                            case "$=":
                                if (LeftField.Type == "string" && RightField.Type == "string")
                                {
                                    return ((string)row[LeftField]).EndsWith((string)row[RightField]) ? 1 : 0;
                                }
                                context.Warn($"The expression {context.Operation.Expression} has an endsWith ($=) which only operates on strings.  Your left field is {LeftField.Type}, and your right field is {RightField.Type}.");
                                return -1;
                            case "*=":
                                if (LeftField.Type == "string" && RightField.Type == "string")
                                {
                                    return ((string)row[LeftField]).Contains((string)row[RightField]) ? 1 : 0;
                                }
                                context.Warn($"The expression {context.Operation.Expression} has a contains (*=) which only operates on strings.  Your left field is {LeftField.Type}, and your right field is {RightField.Type}.");
                                return -1;
                            default:
                                var compare = row[LeftField] as IComparable;
                                if (compare != null)
                                {
                                    return compare.CompareTo(row[RightField]);
                                }
                                return -1;
                        }

                    };
                }
                else
                {
                    context.Warn($"{LeftFieldInput} may not be compared to {RightFieldInput} because they are not the same type!");
                    _compare = (row) => -1;
                }

                TrueField = context.GetAllEntityFields().FirstOrDefault(f => f.Alias == context.Operation.TrueField);
                FalseField = context.GetAllEntityFields().FirstOrDefault(f => f.Alias == context.Operation.FalseField);


            }

            private void ProcessRightSide(IContext context) {
                var rightFieldMatch = _rightSide.Match(context.Operation.Expression);
                RightFieldInput = rightFieldMatch.Success ? rightFieldMatch.Value.Trim() : string.Empty;
                RightField = context.GetAllEntityFields().FirstOrDefault(f => f.Alias == RightFieldInput);
                RightIsField = RightField != null;
            }

            private void ProcessLeftSide(IContext context) {
                var leftFieldMatch = _leftSide.Match(context.Operation.Expression);
                LeftFieldInput = leftFieldMatch.Success ? leftFieldMatch.Value.Trim() : string.Empty;
                LeftField = context.GetAllEntityFields().FirstOrDefault(f => f.Alias == LeftFieldInput);
                LeftIsField = LeftField != null;
            }

        }

        private readonly ExpressionEvaluator _evaluator;

        public IIfTransform(IContext context = null) : base(context, "object") {

            if (IsMissingContext()) {
                return;
            }

            var fields = context.GetAllEntityFields().ToArray();

            if (fields.All(f => f.Alias != context.Operation.TrueField))
            {
                Error($"The iif method's true portion: {context.Operation.TrueField}, is not a valid field.");
                Run = false;
                return;
            }
            if (fields.All(f => f.Alias != context.Operation.FalseField))
            {
                Error($"The iif method's false portion: {context.Operation.FalseField}, is not a valid field.");
                Run = false;
                return;
            }

            _evaluator = new ExpressionEvaluator(context);
        }

        public override IRow Operate(IRow row)
        {
            row[Context.Field] = _evaluator.Evaluate(row);

            return row;
        }

        public override IEnumerable<OperationSignature> GetSignatures() {
            return new[] {
                new OperationSignature("iif") {
                    Parameters = new List<OperationParameter>() {
                        new OperationParameter("expression"),
                        new OperationParameter("true-field"),
                        new OperationParameter("false-field")
                    }
                }
            };
        }

    }
}