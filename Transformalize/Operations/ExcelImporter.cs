using System.Collections.Generic;
using System.IO;
using System.Linq;
using Transformalize.Libs.ExcelDataReader;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Main;

namespace Transformalize.Operations {
    public class ExcelImporter : AbstractOperation {
        private readonly Entity _entity;
        private readonly Field[] _fields;
        private readonly ConversionFactory _conversionFactory = new ConversionFactory();
        private readonly FileInfo _fileInfo;
        private readonly int _start;
        private readonly int _end;

        public ExcelImporter(Entity entity) {
            _entity = entity;
            _fields = new FieldSqlWriter(_entity.Fields).Input().Context().ToEnumerable().OrderBy(f => f.Index).ToArray();
            _fileInfo = new FileInfo(_entity.InputConnection.File);
            _start = _entity.InputConnection.Start;
            _end = entity.InputConnection.End;
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {

            var index = 0;
            var stream = File.Open(_fileInfo.FullName, FileMode.Open, FileAccess.Read);
            var isBinary = _fileInfo.Extension == "xls";

            using (var reader = isBinary ? ExcelReaderFactory.CreateBinaryReader(stream) : ExcelReaderFactory.CreateOpenXmlReader(stream)) {

                if (reader == null)
                    yield break;

                while (reader.Read()) {
                    index++;

                    if (_end > 0 && index > _end) {
                        yield break;
                    }

                    if (index > _start) {
                        var row = new Row();
                        foreach (var field in _fields) {
                            //row[field.Alias] = _conversionFactory.Convert(reader.GetValue(field.Index), field.SimpleType, field.Default);
                            row[field.Alias] = reader.GetValue(field.Index);
                        }
                        yield return row;
                    }
                }
            }
        }
    }
}