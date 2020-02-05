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
using Transformalize.Contracts;

namespace Transformalize.Validators {

   /// <summary>
   /// Itself, the InvertValidator doesn't do anything.  It's here for other validators
   /// to implement it.  For example, if in(map) returns true if the item is in the map, 
   /// then in(map).invert() should return true if the item is not in the map.  It is left
   /// to the previous validator to implement it so the message can be customized.
   /// </summary>
   public class InvertValidator : BaseValidate {

      public InvertValidator(IContext context = null) : base(context) {
         Run = false;
      }

      public override IEnumerable<OperationSignature> GetSignatures() {
         yield return new OperationSignature("invert");
      }

      public override IRow Operate(IRow row) {
         return row;
      }
   }
}