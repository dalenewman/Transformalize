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
using Transformalize.Contracts;

namespace Transformalize.Transforms {

   public class MapTransform : BaseTransform {

      private readonly IField _input;
      private readonly Configuration.Map _operationMap;
      private readonly Dictionary<object, Func<IRow, object>> _map = new Dictionary<object, Func<IRow, object>>();
      private object _catchAll;
      private const string CatchAll = "*";

      public MapTransform(IContext context = null) : base(context, null) {
         if (IsMissingContext()) {
            return;
         }

         if (Context.Operation.Map == string.Empty) {
            Error("The map method requires a map");
            Run = false;
            return;
         }

         _operationMap = Context.Process.Maps.First(m => m.Name == Context.Operation.Map);

         _input = SingleInput();
      }

      public override IEnumerable<IRow> Operate(IEnumerable<IRow> rows) {

         // seems like i have over-complicated this...
         foreach (var item in _operationMap.Items) {
            if (item.From.Equals(CatchAll)) {
               _catchAll = Context.Field.Convert(item.To);
               continue;
            }
            var from = Constants.ObjectConversionMap[Received()](item.From);
            if (item.To == null || item.To.Equals(Constants.DefaultSetting)) {
               if (Context.Entity.TryGetField(item.Parameter, out var field)) {
                  _map[from] = (r) => r[field];
               } else {
                  Context.Error($"Map {Context.Operation.Map} doesn't have a `to` value or a valid field referenced in `parameter`");
                  yield break;
               }
            } else {
               var to = Context.Field.Convert(item.To);
               _map[from] = (r) => to;
            }
         }
         if (_catchAll == null) {
            _catchAll = Context.Field.DefaultValue();
         }

         foreach (var row in rows) {
            yield return Operate(row);
         }
      }

      public override IRow Operate(IRow row) {

         if (_map.TryGetValue(row[_input], out var objects)) {
            row[Context.Field] = objects(row);
         } else {
            row[Context.Field] = _operationMap.PassThrough ? row[_input] : _catchAll;
         }

         return row;
      }

      public override IEnumerable<OperationSignature> GetSignatures() {
         return new[]{
            new OperationSignature("map"){
               Parameters = new List<OperationParameter> {new OperationParameter("map")}
            }
         };
      }
   }

}
