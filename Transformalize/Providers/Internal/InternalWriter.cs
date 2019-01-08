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
using System.Collections.Generic;
using System.Linq;
using Transformalize.Context;
using Transformalize.Contracts;

namespace Transformalize.Provider.Internal {

    public class InternalWriter : IWrite {
        private readonly OutputContext _context;
        private readonly IField[] _fields;
        public InternalWriter(OutputContext context) {
            _context = context;
            _fields = context.Entity.GetAllOutputFields().ToArray();
        }

        public void Write(IEnumerable<IRow> rows) {

            var keys = _fields.Select(f => f.Alias).ToArray();
            var cleared = false;

            foreach (var row in rows) {

                // only clear the output if rows need to be written, otherwise leave it alone
                if (!cleared) {
                    _context.Entity.Rows.Clear();
                    cleared = true;
                }

                _context.Entity.Rows.Add(row.ToCfgRow(_fields, keys));
            }

            if (_context.Entity.Inserts > 0) {
                _context.Info("{0} inserts into {1} {2}", _context.Entity.Inserts, _context.Connection.Name, _context.Entity.Alias);
            }

        }
    }
}
