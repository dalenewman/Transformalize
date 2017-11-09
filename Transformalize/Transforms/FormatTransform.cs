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
using Transformalize.Contracts;

namespace Transformalize.Transforms {

    public class FormatTransform : BaseTransform {

        private readonly BetterFormat _betterFormat;

        public FormatTransform(IContext context = null) : base(context, "string") {
            if (IsMissingContext()) {
                return;
            }
            _betterFormat = new BetterFormat(context, context.Operation.Format, MultipleInput);
            Run = _betterFormat.Valid;
        }

        public override IRow Operate(IRow row) {
            row[Context.Field] = _betterFormat.Format(row);
            Increment();
            return row;
        }

        public new IEnumerable<OperationSignature> GetSignatures() {
            yield return new OperationSignature("format") {
                Parameters = new List<OperationParameter> {
                    new OperationParameter("format")
                }
            };
        }

    }
}