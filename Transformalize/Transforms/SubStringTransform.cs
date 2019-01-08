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
using System.Collections.Generic;
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Transforms {

    public class SubStringTransform : BaseTransform {

        private readonly Field _input;
        private readonly string _type;

        public SubStringTransform(IContext context = null) : base(context, "string") {
            if (IsMissingContext()) {
                return;
            }

            if (IsNotReceiving("string")) {
                return;
            }

            if (Context.Operation.StartIndex == 0 && Context.Operation.Length == 0) {
                Warn($"The substring method in {Context.Field.Alias} has a start index of zero and length of zero, so it will always return an empty string.");
            }

            _input = SingleInput();
            _type = Received();
        }

        public override IRow Operate(IRow row) {
            var value = _type == "string" ? ((string)row[_input]) : row[_input].ToString();
            var len = value.Length;

            if (len <= Context.Operation.StartIndex) {
                row[Context.Field] = string.Empty;

                return row;
            }

            if (Context.Operation.Length == 0 || Context.Operation.StartIndex + Context.Operation.Length > len) {
                row[Context.Field] = value.Substring(Context.Operation.StartIndex);

                return row;
            }

            row[Context.Field] = value.Substring(Context.Operation.StartIndex, Context.Operation.Length);

            return row;
        }

        public override IEnumerable<OperationSignature> GetSignatures() {
            yield return new OperationSignature("substring") {
                Parameters = new List<OperationParameter> {
                    new OperationParameter("start-index"),
                    new OperationParameter("length", "0")
                }
            };
        }

    }
}