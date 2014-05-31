using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Transformalize.Extensions;
using Transformalize.Libs.ExcelDataReader;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Main;
using Transformalize.Main.Providers;

namespace Transformalize.Operations.Extract {

    public class FileExcelExtract : AbstractOperation {
        private readonly Field[] _fields;
        private readonly FileInfo _fileInfo;
        private readonly int _start;
        private readonly int _end;

        public FileExcelExtract(Entity entity, AbstractConnection connection, int top) {
            _fields = new FieldSqlWriter(entity.Fields).Input().Context().OrderedFields().ToArray();
            _fileInfo = new FileInfo(connection.File);
            _start = connection.Start;
            _end = connection.End;

            if (top > 0) {
                _end = _start + top;
            }
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {

            var line = 1;
            var isBinary = _fileInfo.Extension == ".xls";
            using (var fileStream = File.Open(_fileInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {

                using (var reader = isBinary ? ExcelReaderFactory.CreateBinaryReader(fileStream) : ExcelReaderFactory.CreateOpenXmlReader(fileStream)) {

                    if (reader == null) {
                        yield break;
                    }

                    var emptyBuilder = new StringBuilder();

                    while (reader.Read()) {
                        line++;

                        if (line > _start) {
                            if (_end == 0 || line <= _end) {
                                var row = new Row();
                                row["TflFileName"] = _fileInfo.FullName;
                                emptyBuilder.Clear();
                                foreach (var field in _fields) {
                                    var value = reader.GetValue(field.Index);
                                    row[field.Alias] = value;
                                    emptyBuilder.Append(value);
                                }
                                emptyBuilder.Trim(" ");
                                if (!emptyBuilder.ToString().Equals(string.Empty)) {
                                    yield return row;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}