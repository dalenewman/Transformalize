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
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Transforms {

   public class CoalesceTransform : BaseTransform {
      private class FieldWithDefault {
         public Field Field { get; set; }
         public object Default { get; set; }
      }

      private readonly FieldWithDefault[] _input;

      public CoalesceTransform(IContext context = null) : base(context, null) {
         if (IsMissingContext()) {
            return;
         }

         _input = MultipleInput().Select(f => new FieldWithDefault { Field = f, Default = f.Convert(f.Default) }).ToArray();

      }

      public override IRow Operate(IRow row) {
         var first = _input.FirstOrDefault(f => !row[f.Field].Equals(f.Default));
         if (first != null) {
            row[Context.Field] = Context.Field.Type == first.Field.Type ? row[first.Field] : Context.Field.Convert(row[first.Field]);
         }
         return row;
      }

      public override IEnumerable<OperationSignature> GetSignatures() {
         yield return new OperationSignature("coalesce");
      }
   }
}