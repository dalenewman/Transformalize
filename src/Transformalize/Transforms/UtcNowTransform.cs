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
using Transformalize.Contracts;

namespace Transformalize.Transforms {
    public class UtcNowTransform : BaseTransform {

        private readonly Func<DateTime> _now;

        public UtcNowTransform(IContext context = null) : base(context, "datetime") {
            if (IsMissingContext()) {
                return;
            }

            var now = DateTime.UtcNow;

            if (Context.Operation.Count == 0) {
                _now = () => DateTime.UtcNow;
            } else {
                _now = () => now;
            }

        }

        public override IRow Operate(IRow row) {
            row[Context.Field] = _now();
            return row;
        }

        public override IEnumerable<OperationSignature> GetSignatures() {
            return new[] { new OperationSignature("now") { Parameters = new List<OperationParameter>(1) { new OperationParameter("count","0") } } };
        }
    }
}