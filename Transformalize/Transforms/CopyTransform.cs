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
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Transforms {

   public class CopyTransform : BaseTransform {

      private readonly Field _singleInput;
      private readonly Func<IRow, IRow> _transform;

      public CopyTransform(IContext context = null) : base(context, null) {

         if (IsMissingContext()) {
            return;
         }

         Run = false; // by default, it is not actually running a transform, it's just creating parameters for the next operation

         if (IsMissing(Context.Operation.Value)) {
            Error("The copy transform requires at least one parameter.");
            return;
         }

         var nextMethod = NextMethod();

         if (Context.Operation.Value.Contains(",")) {

            if (nextMethod == null) {
               Context.Error($"A copy transform with multiple parameters in {Context.Entity.Alias}.{Context.Field.Alias} must have an operation after it.");
            } else {
               var fields = new List<Field>();
               foreach (var item in Context.Operation.Value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)) {
                  var trial = item.Trim();
                  if (Context.Entity.TryGetField(trial, out var field)) {
                     fields.Add(field);
                  } else {
                     fields.Clear();
                     Context.Error($"Could not find {trial} for copy transform in {Context.Field.Alias}.");
                     break;
                  }
               }
               foreach (var field in fields) {
                  nextMethod.Parameters.Add(new Parameter() { Field = field.Alias });
               }
            }
         } else {

            if (Context.Operation.Value == "*" && nextMethod != null) {
               foreach (var field in Context.Entity.GetAllFields()) {
                  nextMethod.Parameters.Add(new Parameter() { Field = field.Alias });
               }
            } else {
               // simple copy
               if (Context.Entity.TryGetField(Context.Operation.Value, out _singleInput)) {
                  if (nextMethod == null) {
                     Run = true;
                     Returns = _singleInput.Type;
                     _transform = (row) => {
                        row[Context.Field] = row[_singleInput];
                        return row;
                     };
                  } else {
                     nextMethod.Parameters.Add(new Parameter() { Field = _singleInput.Alias });
                  }
               } else {
                  Context.Error($"Could not find {Context.Operation.Value} for copy transform in {Context.Field.Alias}.");
               }
            }
         }
      }

      public override IRow Operate(IRow row) {
         return _transform(row);
      }

      public override IEnumerable<OperationSignature> GetSignatures() {
         yield return new OperationSignature("copy") { Parameters = new List<OperationParameter>(1) { new OperationParameter("value") } };
      }
   }
}