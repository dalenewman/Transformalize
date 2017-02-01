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

using System;
using System.Collections.Generic;
using System.IO;
using FileHelpers;
using FileHelpers.Dynamic;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Extensions;

namespace Transformalize.Provider.File {

    public class DelimitedFileReader : IRead {

        private readonly InputContext _context;
        private readonly IRowFactory _rowFactory;
        private readonly DelimitedClassBuilder _builder;
        private readonly FileInfo _fileInfo;

        public DelimitedFileReader(InputContext context, IRowFactory rowFactory) {
            _context = context;
            _rowFactory = rowFactory;

            var identifier = Utility.Identifier(context.Entity.OutputTableName(context.Process.Name));
            _builder = new DelimitedClassBuilder(identifier) {
                IgnoreEmptyLines = true,
                Delimiter = context.Connection.Delimiter,
                IgnoreFirstLines = context.Connection.Start
            };

            _fileInfo = new FileInfo(_context.Connection.File);

            foreach (var field in context.InputFields) {
                var fieldBuilder = _builder.AddField(field.FieldName(), typeof(string));
                fieldBuilder.FieldQuoted = true;
                fieldBuilder.QuoteChar = _context.Connection.TextQualifier;
                fieldBuilder.QuoteMode = QuoteMode.OptionalForBoth;
                fieldBuilder.FieldOptional = field.Optional;
            }

        }

        public IEnumerable<IRow> Read() {
            FileHelpers.ErrorMode errorMode;
            Enum.TryParse(_context.Connection.ErrorMode, true, out errorMode);

            var engine = new FileHelperAsyncEngine(_builder.CreateRecordClass());
            engine.ErrorManager.ErrorMode = errorMode;
            engine.ErrorManager.ErrorLimit = _context.Connection.ErrorLimit;

            _context.Debug(() => $"Reading {_fileInfo.Name}.");

            var start = _context.Connection.Start;
            var end = 0;
            if (_context.Entity.IsPageRequest()) {
                start += (_context.Entity.Page * _context.Entity.PageSize) - _context.Entity.PageSize;
                end = start + _context.Entity.PageSize;
            }

            var current = _context.Connection.Start;

            using (engine.BeginReadFile(_fileInfo.FullName)) {
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

