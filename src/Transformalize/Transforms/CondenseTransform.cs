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
using System.Collections.Generic;
using Transformalize.Contracts;

namespace Transformalize.Transforms {
    public class CondenseTransform : StringTransform {

        private readonly char _char;
        private readonly IField _input;
        public CondenseTransform(IContext context = null) : base(context, "string") {
            if (IsMissingContext()) {
                return;
            }

            if (IsMissing(Context.Operation.Value)) {
                return;
            }

            if (Context.Operation.Value.Length > 1) {
                Warn("The condense transform can only accept 1 character as a parameter.");
                Run = false;
            }

            _input = SingleInput();

            _char = Context.Operation.Value[0];
        }

        public override IRow Operate(IRow row) {

            var value = GetString(row, _input);
            var found = false;
            var result = new List<char>(value.Length);
            var count = 0;

            foreach (var c in value) {
                if (c == _char) {
                    ++count;
                    switch (count) {
                        case 1:
                            result.Add(c);
                            break;
                        case 2:
                            found = true;
                            break;
                        default:
                            continue;
                    }
                } else {
                    count = 0;
                    result.Add(c);
                }
            }

            row[Context.Field] = found ? string.Concat(result) : value;
            return row;
        }

        public override IEnumerable<OperationSignature> GetSignatures() {
            yield return new OperationSignature("condense") { Parameters = new List<OperationParameter>(1) { new OperationParameter("value", " ") } };
        }
    }
}