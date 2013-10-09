using System;
using System.Collections.Generic;
using Rhino.Etl.Core.Files;
using Transformalize.Libs.FileHelpers.Enums;
using Transformalize.Libs.FileHelpers.RunTime;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Main;
using System.Linq;

namespace Transformalize.Operations {
    public class FileFixedLengthImporter : AbstractOperation {
        private readonly Entity _entity;
        private readonly Field[] _fields;

        public FileFixedLengthImporter(Entity entity) {
            _entity = entity;
            _fields = new FieldSqlWriter(_entity.Fields).Input().Context().ToEnumerable().OrderBy(f => f.Index).ToArray();
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {

            var cb = new FixedLengthClassBuilder("Tfl" + _entity.Alias) { IgnoreEmptyLines = true, FixedMode = FixedMode.AllowVariableLength };
            foreach (var field in _fields) {
                var length = field.Length.Equals("max", StringComparison.OrdinalIgnoreCase) ? Int32.MaxValue : Convert.ToInt32(field.Length);
                cb.AddField(field.Alias, length, typeof(string));
            }

            using (var file = new FluentFile(cb.CreateRecordClass()).From(_entity.InputConnection.File)) {
                foreach (var obj in file) {
                    yield return Row.FromObject(obj);
                }
            }
        }
    }
}