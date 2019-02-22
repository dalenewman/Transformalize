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
using System.IO;
using Transformalize.Context;
using Transformalize.Contracts;

namespace Transformalize.Providers.GeoJson {

    public class GeoJsonFileWriter : IWrite {
        private readonly OutputContext _context;
        private readonly IWrite _streamWriter;
        private readonly MemoryStream _stream;

        public GeoJsonFileWriter(OutputContext context) {
            _context = context;
            _stream = new MemoryStream();
            _streamWriter = new GeoJsonStreamWriter(context, _stream);
        }

        public void Write(IEnumerable<IRow> rows) {
            using (var fileStream = File.Create(_context.Connection.File)) {
                _streamWriter.Write(rows);
                _stream.Seek(0, SeekOrigin.Begin);
                _stream.CopyTo(fileStream);
            }
        }
    }
}