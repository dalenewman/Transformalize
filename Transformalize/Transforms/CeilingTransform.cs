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
    public class CeilingTransform : BaseTransform {
        private readonly Func<IRow, object> _transform;

        public CeilingTransform(IContext context = null) : base(context, "decimal") {
            if (IsMissingContext()) {
                return;
            }

            if (IsNotReceivingNumber()) {
                return;
            }

            var input = SingleInput();
            switch (input.Type) {
                case "decimal":
                    Returns = "decimal";
                    _transform = row => Math.Ceiling((decimal)row[input]);
                    break;
                case "double":
                    Returns = "double";
                    _transform = row => Math.Ceiling((double)row[input]);
                    break;
                default:
                    Returns = "decimal";
                    _transform = row => Math.Floor(Convert.ToDecimal(row[input]));
                    break;

            }
        }

        public override IRow Operate(IRow row) {
            row[Context.Field] = _transform(row);
            Increment();
            return row;
        }


        public override IEnumerable<OperationSignature> GetSignatures() {
            yield return new OperationSignature("ceiling");
        }
    }
}