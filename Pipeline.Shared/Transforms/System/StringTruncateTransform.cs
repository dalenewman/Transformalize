#region license
// Transformalize
// A Configurable ETL Solution Specializing in Incremental Denormalization.
// Copyright 2013 Dale Newman
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
using Pipeline.Configuration;
using Pipeline.Contracts;
using Pipeline.Extensions;

namespace Pipeline.Transforms.System {
    public class StringTruncateTransfom : BaseTransform {
        readonly StringLength[] _strings;

        internal class StringLength : IField {
            public string Alias { get; }
            public short Index { get; set; }
            public short MasterIndex { get; set; }
            public short KeyIndex { get; set; }
            public int Length { get; set; }

            public string Type => "string";

            public StringLength(string alias, short index, short masterIndex, int length) {
                Alias = alias;
                Index = index;
                MasterIndex = masterIndex;
                Length = length;
            }
        }

        public StringTruncateTransfom(IContext context, IEnumerable<Field> fields = null) : base(context, "string") {
            fields = fields ?? context.Entity.GetAllFields();
            _strings = fields.Where(f => f.Type == "string" && f.Length != "max" && f.Output).Select(f => new StringLength(f.Alias, f.Index, f.MasterIndex, Convert.ToInt32(f.Length))).ToArray();
        }

        public override IRow Transform(IRow row) {
            foreach (var field in _strings) {
                row[field] = row[field].ToString().Left(field.Length);
            }
            // Increment();
            return row;
        }
    }
}
