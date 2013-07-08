using System.Collections.Generic;
using Transformalize.Model;
using Transformalize.Rhino.Etl.Core;
using Transformalize.Rhino.Etl.Core.Operations;

namespace Transformalize.Operations
{
    public class EntityDefaults : AbstractOperation {
        private readonly Dictionary<string, Field> _fields;

        public EntityDefaults(Entity entity) {
            _fields = new FieldSqlWriter(entity.All).ExpandXml().Input().Context();
            UseTransaction = false;
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                foreach (var key in row.Columns) {
                    if (row[key] == null) {
                        row[key] = _fields[key].Default;
                    }
                }
                yield return row;
            }
        }
    }
}