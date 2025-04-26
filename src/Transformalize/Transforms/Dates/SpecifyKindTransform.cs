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
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Transforms.Dates {
   public class SpecifyKindTransform : BaseTransform {

      private readonly DateTimeKind _kind;
      private readonly Field _input;

      public SpecifyKindTransform(IContext context = null) : base(context, "datetime") {
         if (IsMissingContext()) {
            return;
         }

         if (IsNotReceiving("date")) {
            return;
         }

         if (IsMissing(Context.Operation.Kind)) {
            return;
         }

         _kind = (DateTimeKind)Enum.Parse(typeof(DateTimeKind), Context.Operation.Kind, true);
         _input = SingleInput();

      }

      public override IRow Operate(IRow row) {
         var dt = (DateTime)row[_input];
         if(dt.Kind != _kind) {
            row[Context.Field] = DateTime.SpecifyKind(dt, _kind);
         }
         return row;
      }

      public override IEnumerable<OperationSignature> GetSignatures() {
         return new[]{ new OperationSignature("specifykind") {
                Parameters = new List<OperationParameter>(1) {
                    new OperationParameter("kind")
                }
            }};
      }
   }
}