﻿#region license
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
using System.IO;
using System.Linq;
using Transformalize.Context;
using Transformalize.Contracts;

namespace Transformalize.Providers.File {
    public class FileStreamWriter : IWrite {

        private readonly OutputContext _context;
        private readonly FileInfo _fileInfo;
        private readonly Stream _stream;

        public FileStreamWriter(OutputContext context) {
            _context = context;
            _fileInfo = new FileInfo(context.Connection.File);
        }

        public FileStreamWriter(OutputContext context, Stream stream) {
            _context = context;
            _stream = stream;
        }

        public void Write(IEnumerable<IRow> rows) {
            var writer = _stream == null ? new StreamWriter(_fileInfo.FullName) : new StreamWriter(_stream);
            var fields = _context.Entity.GetAllOutputFields().Cast<IField>().ToArray();

            using (writer) {

                if (!string.IsNullOrEmpty(_context.Connection.Header)) {
                    writer.WriteLine(_context.Connection.Header);
                }

                foreach (var row in rows) {
                    foreach (var field in fields) {
                        writer.Write(row[field]);
                    }
                    writer.WriteLine();
                }

                if (!string.IsNullOrEmpty(_context.Connection.Footer)) {
                    writer.Write(_context.Connection.Footer);
                }
                writer.Flush();
            }
        }
    }
}