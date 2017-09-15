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
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Extensions;

namespace Transformalize.Providers.File {

    public class DelimitedFileWriter : IWrite {
        private readonly OutputContext _context;
        private readonly string _fileName;

        public DelimitedFileWriter(OutputContext context, string fileName = null) {
            _context = context;
            _fileName = fileName;
        }

        public void Write(IEnumerable<IRow> rows) {

            var engine = FileHelpersEngineFactory.Create(_context);
            var file = Path.Combine(_context.Connection.Folder, _fileName ?? _context.Entity.OutputTableName(_context.Process.Name));
            _context.Debug(() => $"Writing {file}.");

            using (engine.BeginWriteFile(file)) {
                foreach (var row in rows) {
                    for (var i = 0; i < _context.OutputFields.Length; i++) {
                        var field = _context.OutputFields[i];
                        switch (field.Type) {
                            case "byte[]":
                                engine[i] = Convert.ToBase64String((byte[])row[field]);
                                //engine[i] = Utility.BytesToHexString((byte[]) row[field]);
                                //engine[i] = Encoding.UTF8.GetString((byte[])row[field]);
                                break;
                            case "string":
                                engine[i] = row[field];
                                break;
                            case "datetime":
                                engine[i] = ((DateTime)row[field]).ToString("o");
                                break;
                            default:
                                engine[i] = row[field].ToString();
                                break;
                        }
                    }
                    engine.WriteNextValues();
                }
                if (engine.ErrorManager.HasErrors) {
                    var errorFile = Path.Combine(Path.GetDirectoryName(_fileName) ?? string.Empty, Path.GetFileNameWithoutExtension(_fileName) + ".errors.txt");
                    _context.Error($"File writer had {engine.ErrorManager.ErrorCount} error{engine.ErrorManager.ErrorCount.Plural()}. See {errorFile}.");
                    engine.ErrorManager.SaveErrors(errorFile);
                }
            }

        }
    }
}