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
using System.IO;
using System.Text;
using Excel;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Extensions;

namespace Transformalize.Provider.Excel {
    public class ExcelReader : IRead {
        private readonly InputContext _context;
        private readonly IRowFactory _rowFactory;
        private readonly FileInfo _fileInfo;

        public ExcelReader(InputContext context, IRowFactory rowFactory) {
            _fileInfo = new FileInfo(context.Connection.File);
            _context = context;
            _rowFactory = rowFactory;
        }

        public IEnumerable<IRow> Read() {

            // TODO: fix major duplication here

            using (var fileStream = File.Open(_fileInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
                var isBinary = _fileInfo.Extension.ToLower() == ".xls";
                using (var reader = isBinary ? ExcelReaderFactory.CreateBinaryReader(fileStream) : ExcelReaderFactory.CreateOpenXmlReader(fileStream)) {

                    int index = 0;
                    if (reader == null) {
                        yield break;
                    }

                    var start = _context.Connection.Start;
                    var end = 0;
                    if (_context.Entity.IsPageRequest()) {
                        start += (_context.Entity.Page * _context.Entity.PageSize) - _context.Entity.PageSize;
                        end = start + _context.Entity.PageSize;
                    }

                    for (var i = 0; i < start; i++) {
                        reader.Read();
                        ++index;
                    }

                    var readerHasData = false;
                    var emptyDetector = new StringBuilder();

                    if (reader.Read()) {
                        ++index;
                        readerHasData = true;

                        if (end > 0 && end <= index) {
                            _context.Entity.Hits++;
                        } else {
                            emptyDetector.Clear();
                            var row = _rowFactory.Create();
                            for (var i = 0; i < _context.InputFields.Length; i++) {
                                var field = _context.InputFields[i];

                                var expected = Constants.TypeSystem()[field.Type];
                                var actual = reader.IsDBNull(i) ? null : reader.GetValue(i);

                                if (_context.Entity.DataTypeWarnings) {
                                    if (actual != null && expected != actual.GetType()) {
                                        _context.Warn($"The {field.Alias} field in {_context.Entity.Alias} expects a {expected}, but is reading a ({actual.GetType().Name}){actual}.");
                                    }
                                }

                                if (field.Type == "string") {
                                    row[field] = reader.IsDBNull(i) ? null : reader.GetValue(i).ToString();
                                } else {
                                    row[field] = reader.IsDBNull(i) ? null : field.Convert(reader.GetValue(i));
                                }
                                emptyDetector.Append(row[field]);
                            }
                            emptyDetector.Trim(" ");
                            if (!emptyDetector.ToString().Equals(string.Empty)) {
                                _context.Entity.Hits++;
                                yield return row;
                            }

                        }

                    }

                    if (readerHasData) {

                        while (reader.Read()) {

                            ++index;
                            if (end > 0 && end <= index) {
                                _context.Entity.Hits++;
                            } else {
                                emptyDetector.Clear();
                                var row = _rowFactory.Create();
                                for (var i = 0; i < _context.InputFields.Length; i++) {
                                    var field = _context.InputFields[i];
                                    if (field.Type == "string") {
                                        row[field] = reader.IsDBNull(i) ? null : reader.GetString(i);
                                    } else {
                                        row[field] = reader.IsDBNull(i) ? null : field.Convert(reader.GetValue(i));
                                    }
                                    emptyDetector.Append(row[field]);
                                }
                                emptyDetector.Trim(" ");
                                if (!emptyDetector.ToString().Equals(string.Empty)) {
                                    _context.Entity.Hits++;
                                    yield return row;
                                }

                            }

                        }

                    } else {
                        // try read from data set
                        var dr = reader.AsDataSet().CreateDataReader();

                        for (var i = 0; i < start; i++) {
                            dr.Read();
                            ++index;
                        }

                        // check type on first read
                        if (dr.Read()) {

                            ++index;
                            if (end <= index) {
                                _context.Entity.Hits++;
                            } else {
                                emptyDetector.Clear();
                                var row = _rowFactory.Create();
                                for (var i = 0; i < _context.InputFields.Length; i++) {
                                    var field = _context.InputFields[i];

                                    var expected = Constants.TypeSystem()[field.Type];
                                    var actual = dr.GetValue(i).GetType();
                                    if (expected != actual) {
                                        _context.Warn($"The {field.Alias} field in {_context.Entity.Alias} expects a {expected}, but is reading a {actual}.");
                                    }

                                    if (field.Type == "string") {
                                        row[field] = dr.GetValue(i) as string;
                                    } else {
                                        row[field] = field.Convert(dr.GetValue(i));
                                    }
                                    emptyDetector.Append(row[field]);
                                }
                                emptyDetector.Trim(" ");
                                if (!emptyDetector.ToString().Equals(string.Empty)) {
                                    _context.Entity.Hits++;
                                    yield return row;
                                }

                            }

                        }

                        while (dr.Read()) {
                            emptyDetector.Clear();
                            var row = _rowFactory.Create();
                            for (var i = 0; i < _context.InputFields.Length; i++) {
                                var field = _context.InputFields[i];
                                if (field.Type == "string") {
                                    row[field] = dr.GetString(i);
                                } else {
                                    row[field] = field.Convert(dr.GetValue(i));
                                }
                                emptyDetector.Append(row[field]);
                            }
                            emptyDetector.Trim(" ");
                            if (!emptyDetector.ToString().Equals(string.Empty)) {
                                _context.Entity.Hits++;
                                yield return row;
                            }
                        }

                    }

                }
            }

        }
    }
}
