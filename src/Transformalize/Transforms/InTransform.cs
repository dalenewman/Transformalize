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
   public class InTransform : BaseTransform {

      private readonly bool _inMap;
      private readonly Field _input;
      private readonly HashSet<object> _map = new HashSet<object>();

      public InTransform(IContext context = null) : base(context, "bool") {

         if (IsMissingContext()) {
            return;
         }

         if (!Run)
            return;

         var nextOperation = NextOperation();
         _inMap = nextOperation == null || nextOperation.Method != "invert";

         if (Context.Operation.Map == string.Empty) {
            Error("The map method requires a map name or comma delimited list of values.");
            Run = false;
            return;
         }

         _input = SingleInput();
      }

      public override IEnumerable<IRow> Operate(IEnumerable<IRow> rows) {

         /* Override Operate(IEnumerable<IRow>) to load the map, which may not be available at start up */

         foreach (var item in CreateMap().Items) {
            _map.Add(_input.Convert(item.From));
         }

         Func<object, bool> transform;
         if (_inMap) {
            transform = o => _map.Contains(o);
         } else {
            transform = o => !_map.Contains(o);
         }

         foreach(var row in rows) {
            row[Context.Field] = transform(row[_input]);
            yield return row;
         }
      }

      private Map CreateMap() {

         var map = Context.Process.Maps.FirstOrDefault(m => m.Name == Context.Operation.Map);

         if (map != null)
            return map;

         // auto map
         map = new Map { Name = Context.Operation.Map };
         var split = Context.Operation.Map.Split(',');
         foreach (var item in split) {
            map.Items.Add(new MapItem { From = item, To = item });
         }
         Context.Process.Maps.Add(map);

         return map;
      }

      public override IEnumerable<OperationSignature> GetSignatures() {
         yield return new OperationSignature("in") { Parameters = new List<OperationParameter>(1) { new OperationParameter("map") } };
      }

      public override IRow Operate(IRow row) {
         throw new NotImplementedException();
      }
   }
}