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
using System;
using System.Collections.Generic;
using Transformalize.Contracts;

namespace Transformalize.Transforms {

   public class HexTransform : BaseTransform {

      private readonly Func<IRow, object> _transform;

      public HexTransform(IContext context = null) : base(context, "string") {

         if (IsMissingContext()) {
            return;
         }

         var typeReceived = Received();

         if (typeReceived != "byte[]") {
            if (IsNotReceivingNumber()) {
               return;
            }
         }

         var input = SingleInput();

         switch (typeReceived) {
            case "byte[]":
               _transform = row => Utility.BytesToHexViaLookup32((byte[])row[input]);
               break;
            case "float":
            case "decimal":
            case "single":
            case "real":
            case "double":
               Context.Warn("Not converting number with floating point to hex string");
               Run = false;
               break;
            default:
               _transform = row => string.Format("{0:X}", row[input]);
               break;
         }
      }

      public override IRow Operate(IRow row) {
         row[Context.Field] = _transform(row);
         return row;
      }

      public override IEnumerable<OperationSignature> GetSignatures() {
         yield return new OperationSignature("hex");
      }
   }
}