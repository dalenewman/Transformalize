using System;
using System.Collections.Generic;
using System.Linq;
using Rhino.Etl.Core.Files;
using Transformalize.Libs.FileHelpers.Enums;
using Transformalize.Libs.FileHelpers.RunTime;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Main;

namespace Transformalize.Operations.Extract {
    public class FileFixedExtract : AbstractOperation {
        private const StringComparison IC = StringComparison.OrdinalIgnoreCase;
        private readonly Entity _entity;
        private readonly int _top;
        private readonly Field[] _fields;

        public FileFixedExtract(Entity entity, int top) {
            _entity = entity;
            _top = top;
            _fields = new FieldSqlWriter(_entity.Fields).Input().Context().ToEnumerable().OrderBy(f => f.Index).ToArray();
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {

            var cb = new FixedLengthClassBuilder("Tfl" + _entity.Alias) { IgnoreEmptyLines = true, FixedMode = FixedMode.AllowVariableLength };
            foreach (var field in _fields) {
                var length = field.Length.Equals("max", IC) ? Int32.MaxValue : Convert.ToInt32(field.Length);
                cb.AddField(field.Alias, length, typeof(string));
            }

            if (_top > 0) {
                var count = 1;
                using (var file = new FluentFile(cb.CreateRecordClass()).From(_entity.InputConnection.File)) {
                    foreach (var obj in file) {
                        yield return Row.FromObject(obj);
                        count++;
                        if (count == _top) {
                            yield break;
                        }
                    }
                }
            } else {
                using (var file = new FluentFile(cb.CreateRecordClass()).From(_entity.InputConnection.File)) {
                    foreach (var obj in file) {
                        yield return Row.FromObject(obj);
                    }
                }
            }

        }

    }
}