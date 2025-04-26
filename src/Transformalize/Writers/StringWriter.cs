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
using System.Text;
using Transformalize.Context;
using Transformalize.Contracts;

namespace Transformalize.Writers {
    public class StringWriter : IWrite {
        private readonly OutputContext _context;

        public StringWriter(OutputContext context, StringBuilder builder = null) {
            _context = context;
            Builder = builder ?? new StringBuilder();
        }

        public StringBuilder Builder { get; }

        public void Write(IEnumerable<IRow> rows) {
            if (!string.IsNullOrEmpty(_context.Connection.Header)) {
                Builder.AppendLine(_context.Connection.Header);
            }
            var fields = _context.Entity.GetAllOutputFields().Cast<IField>().ToArray();
            foreach (var row in rows) {
                foreach (var field in fields) {
                    Builder.Append(row[field]);
                }
                Builder.AppendLine();
            }
            if (!string.IsNullOrEmpty(_context.Connection.Footer)) {
                Builder.Append(_context.Connection.Footer);
            }
        }
    }
}
