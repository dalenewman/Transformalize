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
using Transformalize.Extensions;

namespace Transformalize.Transforms.System {
   public class StringTruncateTransfom : BaseTransform {
      private readonly StringLength[] _strings;

      public StringTruncateTransfom(IContext context, IEnumerable<Field> fields = null) : base(context, "string") {
         if (Context.Process.ReadOnly) {
            Run = false;
            return;
         }
         fields = fields ?? context.Entity.GetAllFields();
         _strings = fields.Where(f => f.Type == "string" && f.Length != "max" && f.Output && (f.Transforms.Count == 0 || !f.Transforms.Last().ProducesArray)).Select(f => new StringLength(f.Name, f.Alias, f.Index, f.MasterIndex, Convert.ToInt32(f.Length), f.Transforms.Any())).ToArray();
      }

      internal class StringLength : IField {
         public string Name { get; }
         public string Alias { get; }
         public short Index { get; set; }
         public short MasterIndex { get; set; }
         public short KeyIndex { get; set; }
         public int Length { get; set; }
         public string Type => "string";

         public StringLength(string name, string alias, short index, short masterIndex, int length, bool split) {
            Name = name;
            Alias = alias;
            Index = index;
            MasterIndex = masterIndex;
            Length = length;
         }
      }

      public override IRow Operate(IRow row) {
         foreach (var field in _strings) {
            row[field] = row[field].ToString().Left(field.Length);
         }
         return row;
      }

   }
}
