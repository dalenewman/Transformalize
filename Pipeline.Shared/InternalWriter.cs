#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2016 Dale Newman
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
using Pipeline.Configuration;
using Pipeline.Contracts;

namespace Pipeline {

    public class InternalWriter : IWrite {

        private readonly Entity _entity;

        public InternalWriter(Entity entity) {
            _entity = entity;
        }

        public void Write(IEnumerable<IRow> rows) {

            var fields = _entity.GetAllOutputFields().Cast<IField>().ToArray();
            var keys = fields.Select(f => f.Alias).ToArray();
            _entity.Rows.Clear();

            foreach (var row in rows) {
                _entity.Rows.Add(row.ToCfgRow(fields, keys));
            }
        }
    }
}
