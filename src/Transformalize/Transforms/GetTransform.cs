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
   public class GetTransform : BaseTransform {

      private readonly Field _input;
      private readonly int _index;

      public GetTransform(IContext context = null) : base(context, "string") {
         if (IsMissingContext()) {
            return;
         }

         var lastOperation = LastOperation();
         if (lastOperation == null) {
            Error($"The get operation should receive an array. You may want proceed it with a split operation.");
            Run = false;
            return;
         }

         if (!lastOperation.ProducesArray) {
            Error($"The get operation should receive an array. The {lastOperation.Method} is not producing an array.");
            Run = false;
            return;
         }

         if (IsMissing(Context.Operation.Value)) {
            return;
         }

         if (int.TryParse(Context.Operation.Value, out var index)) {
            _index = index;
         } else {
            Context.Error($"The parameter {Context.Operation.Value} in invalid. The get() transform only accepts an integer.");
            Run = false;
         }

         _input = SingleInput();
      }

      public override IRow Operate(IRow row) {
         row[Context.Field] = ((string[])row[_input]).ElementAtOrDefault(_index) ?? string.Empty;
         return row;
      }

      public override IEnumerable<OperationSignature> GetSignatures() {
         yield return new OperationSignature("get") { Parameters = new List<OperationParameter>(1) { new OperationParameter("value") } };
      }
   }
}