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
using System;
using System.Collections.Generic;
using Transformalize.Contracts;

namespace Transformalize.Transforms {

   public class LengthTransform : BaseTransform {

      private readonly Func<IRow, object> _transform;

      public LengthTransform(IContext context = null) : base(context, "int") {

         if (IsMissingContext()) {
            return;
         }

         var input = SingleInput();
         var lastOperation = LastOperation();

         if (lastOperation != null && lastOperation.ProducesArray) {
            _transform = row => ((string[])row[input]).Length;
         } else {
            var typeReceived = Received();

            switch (typeReceived) {
               case "byte[]":
                  _transform = row => ((byte[])row[input]).Length;
                  break;
               default:
                  _transform = row => row[input].ToString().Length;
                  break;
            }
         }
      }

      public override IRow Operate(IRow row) {
         row[Context.Field] = _transform(row);
         return row;
      }

      // this is a workaround: the length parameter is must have the same signature as the length validator
      public override IEnumerable<OperationSignature> GetSignatures() {
         yield return new OperationSignature("length") { Parameters = new List<OperationParameter>(1) { new OperationParameter("length", "0") } };
         yield return new OperationSignature("len");
      }
   }
}