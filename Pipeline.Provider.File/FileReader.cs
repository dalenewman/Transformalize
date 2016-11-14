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
using System.Text;
using Pipeline.Configuration;
using Pipeline.Context;
using Pipeline.Contracts;

namespace Pipeline.Provider.File {
    public class FileReader : IRead {
        private readonly InputContext _context;
        private readonly IRowFactory _rowFactory;
        private readonly Field _field;

        public FileReader(InputContext context, IRowFactory rowFactory) {
            _context = context;
            _rowFactory = rowFactory;
            _field = context.Entity.GetAllFields().First(f => f.Input);
        }

        public IEnumerable<IRow> Read() {
            var encoding = Encoding.GetEncoding(_context.Connection.Encoding);
            var lineNo = 0;
            foreach (var line in System.IO.File.ReadLines(_context.Connection.File, encoding)) {
                ++lineNo;
                if (lineNo < _context.Connection.Start) continue;
                var row = _rowFactory.Create();
                row[_field] = line;
                yield return row;
            }
        }
    }
}