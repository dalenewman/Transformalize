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
using System;
using System.Collections.Generic;
using System.IO;
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Extensions;

namespace Transformalize.Providers.File {

    public class DelimitedFileReader : IRead {

        private readonly InputContext _context;
        private readonly IRowFactory _rowFactory;
        private readonly FileInfo _fileInfo;
        private readonly Field _fileField;

        public DelimitedFileReader(InputContext context, IRowFactory rowFactory) {
            _context = context;
            _rowFactory = rowFactory;
            _fileInfo = new FileInfo(_context.Connection.File);
            context.Entity.TryGetField("TflFile", out _fileField);
        }

        public IEnumerable<IRow> Read() {
            _context.Debug(() => $"Reading {_fileInfo.Name}.");

            var start = _context.Connection.Start;
            var end = 0;
            if (_context.Entity.IsPageRequest()) {
                start += (_context.Entity.Page * _context.Entity.Size) - _context.Entity.Size;
                end = start + _context.Entity.Size;
            }

            var current = _context.Connection.Start;
            var engine = FileHelpersEngineFactory.Create(_context);

            IDisposable reader;
            try {
                reader = engine.BeginReadFile(_fileInfo.FullName);
            } catch (Exception ex) {
                _context.Error(ex.Message);
                yield break;
            }

            using (reader) {
                foreach (var record in engine) {
                    if (end == 0 || current.Between(start, end)) {
                        var values = engine.LastRecordValues;
                        var row = _rowFactory.Create();
                        for (var i = 0; i < _context.InputFields.Length; i++) {
                            var field = _context.InputFields[i];
                            row[field] = values[i];
                        }

                        if (_fileField != null) {
                            row[_fileField] = _context.Connection.File;
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
                    _context.Error($"Error processing line {error.LineNumber} in {_context.Connection.File}.");
                    _context.Warn(error.RecordString.Replace("{","{{").Replace("}","}}"));
                }
            }

        }



    }
}

