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
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Transforms {

    public class PadLeftTransform : BaseTransform {

        private readonly Field _input;

        public PadLeftTransform(IContext context = null): base(context, "string") {

            if (IsMissingContext()){
                return;
            }

            if (IsNotReceiving("string")) {
                return;
            }

            if (context.Operation.TotalWidth == 0) {
                Error("The padleft transform requires total width.");
                Run = false;
                return;
            }

            if (context.Operation.PaddingChar == default(char)) {
                Error("The padleft transform requires a padding character.");
                Run = false;
                return;
            }

            _input = SingleInput();
        }

        public override IRow Operate(IRow row) {
            row[Context.Field] = row[_input].ToString().PadLeft(Context.Operation.TotalWidth, Context.Operation.PaddingChar);
            return row;
        }

        public override IEnumerable<OperationSignature> GetSignatures() {
            yield return new OperationSignature("padleft") {
                Parameters = new List<OperationParameter> {
                    new OperationParameter("total-width"),
                    new OperationParameter("padding-char","0")
                }
            };
        }

    }
}