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
using System.Collections.Generic;
using Transformalize.Contracts;
using Transformalize.Extensions;

namespace Transformalize.Transforms {
   public class EllipsisTransform : BaseTransform {
      private readonly int _length;
      private readonly string _ellipsis;
      private readonly IField _input;

      public EllipsisTransform(IContext context = null) : base(context, "string") {
         if (IsMissingContext()) {
            return;
         }
         if (IsNotReceiving("string")) {
            return;
         }

         if (Context.Operation.Length == 0) {
            Error("The ellipsis transform requires a length parameter.");
            Run = false;
            return;
         }

         _ellipsis = Context.Operation.Ellipsis;
         _length = Context.Operation.Length;
         _input = SingleInput();
      }

      public override IRow Operate(IRow row) {
         var content = row[_input].ToString();
         row[Context.Field] = content.Length > _length ? content.Left(_length).TrimEnd(' ') + _ellipsis : content;
         return row;
      }

      public override IEnumerable<OperationSignature> GetSignatures() {
         return new[]{
                new OperationSignature("ellipsis"){
                    Parameters = new List<OperationParameter> {
                       new OperationParameter("length"),
                       new OperationParameter("ellipsis", "...")
                    }
                }
            };
      }
   }
}