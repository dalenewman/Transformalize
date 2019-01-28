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
using Transformalize.Contracts;

namespace Transformalize.Transforms {
    public class AddTransform : BaseTransform {
        private readonly Func<IRow, object> _transform;

        public AddTransform(IContext context = null) : base(context, "decimal") {
            if (IsMissingContext()) {
                return;
            }

            if (IsNotReceivingNumbers()) {
                return;
            }

            var values = new List<string>();

            if (Context.Operation.Value != Constants.DefaultSetting) {
                if (Context.Operation.Separator != Constants.DefaultSetting) {
                    foreach (var value in Context.Operation.Value.Split(Context.Operation.Separator[0])) {
                        if (double.TryParse(value, out var v)) {
                            values.Add(value);
                        }
                    }
                } else {
                    if (double.TryParse(Context.Operation.Value, out var v)) {
                        values.Add(Context.Operation.Value);
                    }
                }
            }

            var input = MultipleInput();
            var same = input.All(i => i.Type == input.First().Type);
            if (same) {
                var type = input.First().Type;
                switch (type) {
                    case "decimal":
                        Returns = "decimal";
                        if (values.Any()) {
                            decimal add = 0;
                            foreach (var value in values) {
                                if (decimal.TryParse(value, out var result)) {
                                    add += result;
                                }
                            }
                            _transform = row => input.Sum(f => (decimal)row[f]) + add;
                        } else {
                            _transform = row => input.Sum(f => (decimal)row[f]);
                        }
                        break;
                    case "double":
                        Returns = "double";
                        if (values.Any()) {
                            double add = 0;
                            foreach (var value in values) {
                                if (double.TryParse(value, out var result)) {
                                    add += result;
                                }
                            }
                            _transform = row => input.Sum(f => (double)row[f]) + add;
                        } else {
                            _transform = row => input.Sum(f => (double)row[f]);
                        }
                        break;
                    case "long":
                    case "int64":
                        Returns = "long";
                        if (values.Any()) {
                            long add = 0;
                            foreach (var value in values) {
                                if (long.TryParse(value, out var result)) {
                                    add += result;
                                }
                            }
                            _transform = row => input.Sum(f => (long)row[f]) + add;
                        } else {
                            _transform = row => input.Sum(f => (long)row[f]);
                        }
                        break;
                    case "int":
                    case "int32":
                        Returns = "int";
                        if (values.Any()) {
                            int add = 0;
                            foreach (var value in values) {
                                if (int.TryParse(value, out var result)) {
                                    add += result;
                                }
                            }
                            _transform = row => input.Sum(f => (int)row[f]) + add;
                        } else {
                            _transform = row => input.Sum(f => (int)row[f]);
                        }
                        break;
                    default:
                        if (values.Any()) {
                            decimal add = 0;
                            foreach (var value in values) {
                                if (decimal.TryParse(value, out var result)) {
                                    add += result;
                                }
                            }
                            _transform = row => input.Sum(f => Convert.ToDecimal(row[f])) + add;
                        } else {
                            _transform = row => input.Sum(f => Convert.ToDecimal(row[f]));
                        }
                        break;
                }
            } else {
                _transform = row => input.Sum(field => field.Type == "decimal" ? (decimal)row[field] : Convert.ToDecimal(row[field]));
            }
        }

        public override IRow Operate(IRow row) {
            row[Context.Field] = _transform(row);
            return row;
        }

        public new IEnumerable<OperationSignature> GetSignatures() {
            yield return new OperationSignature("add") { Parameters = new List<OperationParameter>(1) { new OperationParameter("value") } };
            yield return new OperationSignature("sum") { Parameters = new List<OperationParameter>(1) { new OperationParameter("value") } };
        }

    }
}