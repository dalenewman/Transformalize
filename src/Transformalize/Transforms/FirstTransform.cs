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
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Transforms {
    public class FirstTransform : BaseTransform {
        private readonly Field _input;

        public FirstTransform(IContext context = null) : base(context, "string") {
            if (IsMissingContext()) {
                return;
            }

         var lastOperation = LastOperation();
         if (lastOperation == null) {
            Error($"The first operation should receive an array. You may want proceed it with a split operation.");
            Run = false;
            return;
         }

         if (!lastOperation.ProducesArray) {
            Error($"The first operation should receive an array. The {lastOperation.Method} is not producing an array.");
            Run = false;
            return;
         }

         _input = SingleInput();
        }

        public override IRow Operate(IRow row) {
            row[Context.Field] = ((string[])row[_input]).FirstOrDefault() ?? string.Empty;
            
            return row;
        }

        public override IEnumerable<OperationSignature> GetSignatures() {
            yield return new OperationSignature("first");
        }
    }
}