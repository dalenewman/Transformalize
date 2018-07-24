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

using System.Collections.Generic;
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Transforms {

    public class PrependTransform : StringTransform {

        private readonly Field _input;
        private readonly Field _field;

        public PrependTransform(IContext context = null) : base(context, "string") {
            if (IsMissingContext()) {
                return;
            }

            _input = SingleInput();

            Context.Entity.TryGetField(Context.Operation.Value, out _field);

            if (Received() != "string") {
                Warn($"Prepending to a {Received()} type in {Context.Field.Alias} converts it to a string.");
            }
        }

        public override IRow Operate(IRow row) {
            row[Context.Field] = (_field == null ? Context.Operation.Value : GetString(row, _field)) + row[_input];
            return row;
        }

        public override IEnumerable<OperationSignature> GetSignatures() {
            return new[] {
                new OperationSignature("prepend") {
                    Parameters = new List<OperationParameter> {new OperationParameter("value")}
                },
                new OperationSignature("prefix") {
                    Parameters = new List<OperationParameter> {new OperationParameter("value")}
                }
            };
        }
    }

}