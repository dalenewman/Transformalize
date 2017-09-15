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
using System.Linq;
using OpenXmlPowerTools;
using Transformalize.Configuration;
using Transformalize.Contracts;
using Transformalize.Extensions;

namespace Transformalize.Providers.OpenXml {
    public class ExcelWriter : IWrite {

        private readonly WorkbookDfn _workbook;
        private readonly FileInfo _fileInfo;
        private readonly Field[] _fields;
        private readonly List<RowDfn> _rowDfns;

        public ExcelWriter(IConnectionContext context) {
            _fileInfo = new FileInfo(context.Connection.File);
            _fields = context.Entity.GetAllOutputFields().Where(f => !f.System).ToArray();
            _rowDfns = new List<RowDfn>();

            _workbook = new WorkbookDfn {
                Worksheets = new[] {new WorksheetDfn {
                Name = context.Entity.Alias.Left(32),
                ColumnHeadings = _fields.Select(field =>
                    new CellDfn {
                        Value = field.Label,
                        Bold = true,
                        HorizontalCellAlignment = HorizontalCellAlignment.Center
                    }
                ),
                Rows = _rowDfns
                }
            }};
        }

        public void Write(IEnumerable<IRow> rows) {

            foreach (var row in rows) {
                var cellDfns = new List<CellDfn>();
                var rowDef = new RowDfn { Cells = cellDfns };
                cellDfns.AddRange(_fields.Select(field => new CellDfn {
                    CellDataType = field.ToCellDataType(),
                    Value = field.Type == "guid" ? row[field].ToString() : row[field],
                    FormatCode = field.Format.Replace("tt","AM/PM")
                }));
                _rowDfns.Add(rowDef);
            }

            SpreadsheetWriter.Write(_fileInfo.FullName, _workbook);
        }

    }
}
