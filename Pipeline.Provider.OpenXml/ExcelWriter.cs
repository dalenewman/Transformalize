using System.Collections.Generic;
using System.IO;
using System.Linq;
using Pipeline.Contracts;
using OpenXmlPowerTools;
using Pipeline.Configuration;

namespace Pipeline.Provider.OpenXml {
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
                Name = context.Entity.Alias,
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
                    Value = row[field]
                }));
                _rowDfns.Add(rowDef);
            }

            SpreadsheetWriter.Write(_fileInfo.FullName, _workbook);
        }

    }
}
