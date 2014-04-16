using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            _fields = new FieldSqlWriter(entity.Fields).Input().Context().ToEnumerable().OrderBy(f => f.Index).ToArray();
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

                    while (reader.Read()) {
                        line++;

                        if (line > _start) {
                            if (_end == 0 || line <= _end){
                                var row = new Row();
                                row["TflFileName"] = _fileInfo.FullName;
                                foreach (var field in _fields) {
                                    row[field.Alias] = reader.GetValue(field.Index);
                                }
                                yield return row;
                            }
                        }
                    }
                }
            }
        }
    }
}