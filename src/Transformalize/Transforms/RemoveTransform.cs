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
    public class RemoveTransform : BaseTransform {
        private readonly Field _input;

        public RemoveTransform(IContext context = null) : base(context, "string") {
            if (IsMissingContext()) {
                return;
            }

            if (IsNotReceiving("string")) {
                return;
            }

            if (Context.Operation.StartIndex == 0) {
                Error("The remove transform requires a start-index greater than 0.");
                Run = false;
                return;
            }

            _input = SingleInput();
        }

        public override IRow Operate(IRow row) {
            row[Context.Field] = Context.Operation.Count > 0
                ? row[_input].ToString().Remove(Context.Operation.StartIndex, Context.Operation.Count)
                : row[_input].ToString().Remove(Context.Operation.StartIndex);
            
            return row;
        }

        public override IEnumerable<OperationSignature> GetSignatures() {
            yield return new OperationSignature("remove") {
                Parameters = new List<OperationParameter> {
                    new OperationParameter("start-index"),
                    new OperationParameter("count","0")
                }
            };
        }
    }
}