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
using System.Text.Json;
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Transforms.Json {

   public class ToJsonTransform : BaseTransform {

      private readonly Field[] _input;
      private readonly JsonSerializerOptions _options;
      public ToJsonTransform(IContext context = null) : base(context, "string") {

         if (IsMissingContext()) {
            return;
         }

         _input = MultipleInput().Where(f=>f.Alias != Context.Field.Alias).ToArray();

         if (!_input.Any()) {
            Error($"The {Context.Operation.Method} transform requires input (e.g. copy(something)).");
            Run = false;
            return;
         }

         _options = new JsonSerializerOptions { WriteIndented = Context.Operation.Format.ToLower() == "indented" };
      }

      public override IRow Operate(IRow row) {

         var dict = new Dictionary<string, object>();

         foreach (var field in _input) {
            dict[field.Alias] = row[field];
         }

         row[Context.Field] = JsonSerializer.Serialize(dict, _options);
         return row;
      }

      public override IEnumerable<OperationSignature> GetSignatures() {
         yield return new OperationSignature("tojson") {
            Parameters = new List<OperationParameter>(1) {
               new OperationParameter("format", "none")
            }
         };
      }

   }

}
