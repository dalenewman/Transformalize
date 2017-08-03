#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2017 Dale Newman
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
using Transformalize.Context;
using Transformalize.Contracts;

namespace Transformalize.Provider.Internal {

    public class InternalWriter : IWrite {
        private readonly OutputContext _context;

        public InternalWriter(OutputContext context) {
            _context = context;
        }

        public void Write(IEnumerable<IRow> rows) {

            var fields = _context.Entity.GetAllOutputFields().Cast<IField>().ToArray();
            var keys = fields.Select(f => f.Alias).ToArray();
            _context.Entity.Rows.Clear();

            foreach (var row in rows) {
                _context.Entity.Rows.Add(row.ToCfgRow(fields, keys));
                _context.Increment();
            }

            if (_context.Entity.Inserts > 0) {
                _context.Info("{0} inserts into {1} {2}", _context.Entity.Inserts, _context.Connection.Name, _context.Entity.Alias);
            }

        }
    }
}
