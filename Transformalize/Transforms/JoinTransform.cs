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
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Transforms {
   public class JoinTransform : BaseTransform {
      private readonly Field[] _input;
      private readonly Field _single;
      private readonly Func<IRow, string> _transform;
      private bool _passed = false;

      public JoinTransform(IContext context = null) : base(context, "string") {

         if (IsMissingContext()) {
            return;
         }
         if (IsMissing(Context.Operation.Separator)) {
            return;
         }
         _input = MultipleInput();

         var lastOperation = LastOperation();
         if(_input.Length == 1 && lastOperation != null && lastOperation.ProducesArray) {
            _single = _input.First();
            _transform = row => string.Join(Context.Operation.Separator, (string[])row[_single]);
         } else {
            _transform = row => string.Join(Context.Operation.Separator, _input.Select(f => row[f]));
         }
      }
      public override IRow Operate(IRow row) {
         row[Context.Field] = _transform(row);
         return row;
      }

      public override IEnumerable<IRow> Operate(IEnumerable<IRow> rows) {
         if (Run) {
            foreach(var row in rows) {
               if (_passed) {
                  yield return Operate(row);
               } else {
                  IRow result = row;
                  try {
                     result = Operate(row);
                     _passed = true;
                  } catch (InvalidCastException ex) {
                     Error(ex.Message);
                  }
                  yield return result;
               }
            }
         } else {
            foreach(var row in rows) {
               yield return row;
            }
         }
      }

      public override IEnumerable<OperationSignature> GetSignatures() {
         return new[] {
                new OperationSignature("join") {
                    Parameters = new List<OperationParameter> {new OperationParameter("separator", ",")}
                }
            };
      }
   }
}
