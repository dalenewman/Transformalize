using System.Collections.Generic;
using System.Linq;
using Rhino.Etl.Core.Files;
using Transformalize.Libs.FileHelpers.RunTime;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Main;

namespace Transformalize.Operations
{
    public class FileDelimitedImporter : AbstractOperation
    {
        private readonly Entity _entity;
        private readonly Field[] _fields;
        private readonly string _delimiter ;

        public FileDelimitedImporter(Entity entity) {
            _entity = entity;
            _fields = new FieldSqlWriter(_entity.Fields).Input().Context().ToEnumerable().OrderBy(f => f.Index).ToArray();
            _delimiter = _entity.InputConnection.Delimiter;
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {

            var cb = new DelimitedClassBuilder("Tfl" + _entity.Alias) { IgnoreEmptyLines = true, Delimiter = _delimiter};
            foreach (var field in _fields) {
                cb.AddField(field.Alias, field.SystemType);
            }

            using (var file = new FluentFile(cb.CreateRecordClass()).From(_entity.InputConnection.File)) {
                foreach (var obj in file) {
                    yield return Row.FromObject(obj);
                }
            }
        }

        
    }
}