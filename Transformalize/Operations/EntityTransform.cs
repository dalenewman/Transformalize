using System.Collections.Generic;
using Transformalize.Model;
using Transformalize.Rhino.Etl.Core;
using Transformalize.Rhino.Etl.Core.Operations;

namespace Transformalize.Operations {
    public class EntityTransform : AbstractOperation {
        private readonly Dictionary<string, Field> _fields;

        public EntityTransform(Entity entity) {
            _fields = new FieldSqlWriter(entity.All).ExpandXml().HasTransform().Input().Context();
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {

                foreach (var key in _fields.Keys) {
                    var field = _fields[key];
                    if (field.UseStringBuilder) {
                        field.StringBuilder.Clear();
                        field.StringBuilder.Append(row[key]);
                        foreach (var t in field.Transforms) {
                            t.Transform(ref field.StringBuilder);
                        }
                        row[key] = field.StringBuilder.ToString();
                    } else {
                        var value = row[key];
                        foreach (var t in field.Transforms) {
                            t.Transform(ref value);
                        }
                        row[key] = value;
                    }
                }
                yield return row;
            }
        }
    }

}