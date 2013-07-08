using System.Collections.Generic;
using Transformalize.Model;
using Transformalize.Rhino.Etl.Core;
using Transformalize.Rhino.Etl.Core.Operations;

namespace Transformalize.Operations {
    public class EntityTransform : AbstractOperation {
        private readonly Dictionary<string, Field> _fields;

        public EntityTransform(Entity entity) {
            _fields = new FieldSqlWriter(entity.All).ExpandXml().HasTransform().Input().Context();
            UseTransaction = false;
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {

                foreach (var pair in _fields) {
                    var value = row[pair.Key];

                    if (pair.Value.UseStringBuilder) {
                        pair.Value.StringBuilder.Clear();
                        pair.Value.StringBuilder.Append(value);
                        foreach (var t in pair.Value.Transforms) {
                            t.Transform(ref pair.Value.StringBuilder);
                        }
                        row[pair.Key] = pair.Value.StringBuilder.ToString();
                    } else {
                        foreach (var t in pair.Value.Transforms) {
                            t.Transform(ref value);
                        }
                        row[pair.Key] = value;
                    }
                }
                yield return row;
            }
        }
    }

}