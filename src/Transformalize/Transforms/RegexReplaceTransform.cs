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
using System.Text.RegularExpressions;
using Transformalize.Contracts;

namespace Transformalize.Transforms {
    public class RegexReplaceTransform : BaseTransform {

        private readonly Action<IRow> _transform;

        public RegexReplaceTransform(IContext context = null) : base(context, "string") {

            if (IsMissingContext()) {
                return;
            }

            if (IsNotReceiving("string")) {
                return;
            }
            
            if (IsMissing(context.Operation.Pattern)) {
                return;
            }

            var input = SingleInput();
#if NETS10
            var regex = new Regex(context.Operation.Pattern);
#else
            var regex = new Regex(context.Operation.Pattern, RegexOptions.Compiled);
#endif
            if (context.Operation.Count == 0) {
                _transform = r => r[Context.Field] = regex.Replace(r[input].ToString(), context.Operation.NewValue);
            } else {
                _transform = r => r[Context.Field] = regex.Replace(r[input].ToString(), context.Operation.NewValue, context.Operation.Count);
            }
        }

        public override IRow Operate(IRow row) {
            _transform(row);
            
            return row;
        }

        public override IEnumerable<OperationSignature> GetSignatures() {
            yield return new OperationSignature("regexreplace") {
                Parameters = new List<OperationParameter> {
                    new OperationParameter("pattern"),
                    new OperationParameter("new-value"),
                    new OperationParameter("count","0")
                }
            };
        }
    }
}