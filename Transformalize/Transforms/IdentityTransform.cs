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

    public class IdentityTransform : BaseTransform {

        private int _seed;
        private readonly int _step;

        public IdentityTransform(IContext context = null) : base(context, "int") {
            if (IsMissingContext()) {
                return;
            }

            _seed = Context.Operation.Seed;
            _step = Context.Operation.Step;
        }

        public override IRow Operate(IRow row) {
            row[Context.Field] = _seed;
            _seed = _seed + _step;
            return row;
        }

        public override IEnumerable<OperationSignature> GetSignatures(){
            yield return new OperationSignature("identity") {
                Parameters = new List<OperationParameter>(2) {
                    new OperationParameter("seed", "1"),
                    new OperationParameter("step", "1")
                }
            };
        }
    }
}