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
using System.IO;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Extensions;

namespace Transformalize.Providers.File {

    public class DelimitedFileStreamReader : IRead {

        private readonly InputContext _context;
        private readonly Stream _stream;
        private readonly IRowFactory _rowFactory;

        public DelimitedFileStreamReader(InputContext context, Stream stream, IRowFactory rowFactory) {
            _context = context;
            _stream = stream;
            _rowFactory = rowFactory;
        }

        public IEnumerable<IRow> Read() {

            _context.Debug(() => "Reading file stream.");

            var start = _context.Connection.Start;
            var end = 0;
            if (_context.Entity.IsPageRequest()) {
                start += (_context.Entity.Page * _context.Entity.PageSize) - _context.Entity.PageSize;
                end = start + _context.Entity.PageSize;
            }

            var current = _context.Connection.Start;

            var engine = FileHelpersEngineFactory.Create(_context);

            using (engine.BeginReadStream(new StreamReader(_stream))) {
                foreach (var record in engine) {
                    if (end == 0 || current.Between(start, end)) {
                        var values = engine.LastRecordValues;
                        var row = _rowFactory.Create();
                        for (var i = 0; i < _context.InputFields.Length; i++) {
                            var field = _context.InputFields[i];
                            if (field.Type == "string") {
                                row[field] = values[i] as string;
                            } else {
                                row[field] = field.Convert(values[i]);
                            }
                        }
                        yield return row;
                    }
                    ++current;
                    if (current == end) {
                        break;
                    }
                }
            }

            if (engine.ErrorManager.HasErrors) {
                foreach (var error in engine.ErrorManager.Errors) {
                    _context.Error(error.ExceptionInfo.Message);
                }
            }

        }
    }
}

