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
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Transforms.DateMath {
    public class TimeZoneOffsetTransform : BaseTransform {

        private readonly Field _input;
        private readonly Func<DateTime, int> _transform;

        public TimeZoneOffsetTransform(IContext context = null) : base(context, "int") {
            if (IsMissingContext()) {
                return;
            }

            if (IsNotReceiving("date")) {
                return;
            }

            if (IsMissing(Context.Operation.FromTimeZone)) {
                return;
            }

            if (IsMissing(Context.Operation.ToTimeZone)) {
                return;
            }

            _input = SingleInput();

            var fromTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(Context.Operation.FromTimeZone);
            var toTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(Context.Operation.ToTimeZone);

            var adjustment = toTimeZoneInfo.BaseUtcOffset - fromTimeZoneInfo.BaseUtcOffset;
            var daylightAdjustment = adjustment.Add(new TimeSpan(0, 1, 0, 0));
            _transform = dt => toTimeZoneInfo.IsDaylightSavingTime(dt) ? daylightAdjustment.Hours : adjustment.Hours;

        }

        public override IRow Operate(IRow row) {
            var date = (DateTime)row[_input];
            row[Context.Field] = _transform(date);
            return row;
        }

        public override IEnumerable<OperationSignature> GetSignatures() {
            return new[]{ new OperationSignature("timezoneoffset") {
                Parameters = new List<OperationParameter>(2) {
                    new OperationParameter("from-time-zone"),
                    new OperationParameter("to-time-zone")
                }
            }};
        }
    }
}