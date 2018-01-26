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
using System.Collections.Generic;
using Transformalize.Contracts;

namespace Transformalize.Transforms {
    public class NextTransform : BaseTransform {
        private readonly DateTime _next;

        public NextTransform(IContext context = null) : base(context, "datetime") {

            if (IsMissingContext()) {
                return;
            }

            if (IsNotReceiving("date")) {
                return;
            }

            if (IsMissing(Context.Operation.DayOfWeek)) {
                return;
            }

            var from = DateTime.Today;
            var to = Enum.Parse(typeof(DayOfWeek), Context.Operation.DayOfWeek, true);
            var start = (int)from.DayOfWeek;
            var target = (int)to;
            if (target <= start) {
                target += 7;
            }

            _next = from.AddDays(target - start);
        }

        public override IRow Operate(IRow row) {
            row[Context.Field] = _next;
            
            return row;
        }

        public override IEnumerable<OperationSignature> GetSignatures() {
            return new[] {
                new OperationSignature("next") {
                    Parameters = new List<OperationParameter> {new OperationParameter("dayofweek")}
                }
            };
        }
    }
}