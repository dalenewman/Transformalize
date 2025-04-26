#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2025 Dale Newman
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
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Transforms {

    public class DateAddTransform : BaseTransform {

        private readonly Field _referencedField;
        private readonly Func<IRow, object> _transform;

        public DateAddTransform(IContext context = null) : base(context, "datetime") {

            if (IsMissingContext()) {
                return;
            }

            if (IsNotReceiving("date")) {
                return;
            }

            TimeSpan amount;
            var input = SingleInput();

            if (!double.TryParse(Context.Operation.Value, out var value)) {
                if (Context.Entity.TryGetField(Context.Operation.Value, out _referencedField)) {

                } else {
                    Error($"The {Context.Operation.Method} transform requires a double numeric value or field reference containing such a value.  {Context.Operation.Value} can not be parsed as a double and is not a field reference.");
                    Run = false;
                    return;
                }

            }

            switch (Context.Operation.TimeComponent.ToLower()) {
                case "s":
                case "second":
                case "seconds":
                    amount = TimeSpan.FromSeconds(value);
                    _transform = row => ((DateTime)row[input]).Add(TimeSpan.FromSeconds(Convert.ToDouble(row[_referencedField])));
                    break;
                case "ms":
                case "millisecond":
                case "milliseconds":
                    amount = TimeSpan.FromMilliseconds(value);
                    _transform = row => ((DateTime)row[input]).Add(TimeSpan.FromMilliseconds(Convert.ToDouble(row[_referencedField])));
                    break;
                case "day":
                case "days":
                    amount = TimeSpan.FromDays(value);
                    _transform = row => ((DateTime)row[input]).Add(TimeSpan.FromDays(Convert.ToDouble(row[_referencedField])));
                    break;
                case "hour":
                case "hours":
                    amount = TimeSpan.FromHours(value);
                    _transform = row => ((DateTime)row[input]).Add(TimeSpan.FromHours(Convert.ToDouble(row[_referencedField])));
                    break;
                case "minute":
                case "minutes":
                    amount = TimeSpan.FromMinutes(value);
                    _transform = row => ((DateTime)row[input]).Add(TimeSpan.FromMinutes(Convert.ToDouble(row[_referencedField])));
                    break;
                case "tick":
                case "ticks":
                    amount = TimeSpan.FromTicks(Convert.ToInt64(Context.Operation.Value));
                    if (!long.TryParse(Context.Operation.Value, out _)) {
                        Error($"The dateadd ticks transform requires a long (int64) numeric parameter.  {Context.Operation.Value} can not be parsed as a long.");
                        Run = false;
                    }
                    break;
                default:
                    Context.Warn($"Add time does not support {Context.Operation.TimeComponent}. No time being added");
                    amount = new TimeSpan();
                    Run = false;
                    break;

            }

            if (_referencedField == null) {
                _transform = (row) => ((DateTime)row[input]).Add(amount);
            }
        }

        public override IRow Operate(IRow row) {
            row[Context.Field] = _transform(row);
            return row;
        }

        public override IEnumerable<OperationSignature> GetSignatures() {
            return new[] {
                new OperationSignature("dateadd") {
                    Parameters = new List<OperationParameter>(2) {
                        new OperationParameter("value"),
                        new OperationParameter("time-component", "days")
                    }
                }
            };
        }
    }
}