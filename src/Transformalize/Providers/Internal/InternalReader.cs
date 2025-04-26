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
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Transforms;

namespace Transformalize.Providers.Internal {

   public class InternalReader : IRead {

      private readonly InputContext _input;
      private readonly IRowFactory _rowFactory;
      private readonly HashSet<string> _missing;
      private readonly List<ITransform> _transforms = new List<ITransform>();
      private readonly Field[] _fields;

      public InternalReader(InputContext input, IRowFactory rowFactory) : this(input, input.Entity.Fields, rowFactory) { }

      public InternalReader(InputContext input, IEnumerable<Field> fields, IRowFactory rowFactory) {
         _input = input;
         _rowFactory = rowFactory;
         _missing = new HashSet<string>();
         _fields = fields.Where(f=>f.Input).ToArray();

         foreach (var field in _fields.Where(f => f.Type != "string" && (!f.Transforms.Any() || f.Transforms.First().Method != "convert"))) {
            _transforms.Add(new ConvertTransform(new PipelineContext(input.Logger, input.Process, input.Entity, field, new Operation { Method = "convert" })));
         }
      }

      private IEnumerable<IRow> PreRead() {

         var rows = new List<IRow>();
         foreach (var row in _input.Entity.Rows) {

            var stringRow = _rowFactory.Create();
            foreach (var field in _fields) {
               if (row.Map.ContainsKey(field.Name)) {
                  stringRow[field] = row[field.Name];
               } else {
                  if (_missing.Add(field.Name)) {
                     _input.Warn($"An internal row in {_input.Entity.Alias} is missing the field {field.Name}.");
                  }
               }
            }
            rows.Add(stringRow);
         }
         _input.Entity.Hits = rows.Count;
         return rows;
      }

      public IEnumerable<IRow> Read() {
         return _transforms.Aggregate(PreRead(), (rows, transform) => transform.Operate(rows));
      }

      public object GetVersion() {
         return null;
      }
   }
}