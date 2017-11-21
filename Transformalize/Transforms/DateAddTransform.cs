#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2017 Dale Newman
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
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Transforms {

    public class DateAddTransform : BaseTransform {

        private readonly Field _input;
        private readonly TimeSpan _amount;

        public DateAddTransform(IContext context) : base(context, "datetime") {

            if (IsNotReceiving("date")) {
                return;
            }

            _input = SingleInput();

            if (!double.TryParse(context.Operation.Value, out double value)) {
                Error($"The {context.Operation.Method} transform requires a double numeric parameter.  {context.Operation.Value} can not be parsed as a double.");
                Run = false;
                return;
            }

            switch (context.Operation.TimeComponent.ToLower()) {
                case "second":
                case "seconds":
                    _amount = TimeSpan.FromSeconds(value);
                    break;
                case "millisecond":
                case "milliseconds":
                    _amount = TimeSpan.FromMilliseconds(value);
                    break;
                case "day":
                case "days":
                    _amount = TimeSpan.FromDays(value);
                    break;
                case "hour":
                case "hours":
                    _amount = TimeSpan.FromHours(value);
                    break;
                case "minute":
                case "minutes":
                    _amount = TimeSpan.FromMinutes(value);
                    break;
                case "tick":
                case "ticks":
                    _amount = TimeSpan.FromTicks(Convert.ToInt64(context.Operation.Value));
                    long addTicksLong;
                    if (!long.TryParse(context.Operation.Value, out addTicksLong)) {
                        Error($"The dateadd ticks transform requires a long (int64) numeric parameter.  {context.Operation.Value} can not be parsed as a long.");
                    }
                    break;
                default:
                    context.Warn($"Add time does not support {context.Operation.TimeComponent}. No time being added");
                    _amount = new TimeSpan();
                    break;

            }
        }

        public override IRow Operate(IRow row) {
            row[Context.Field] = ((DateTime)row[_input]).Add(_amount);
            Increment();
            return row;
        }

    }
}