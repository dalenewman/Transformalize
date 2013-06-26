using System.Collections.Generic;
using System.Linq;
using Transformalize.Model;
using Transformalize.Rhino.Etl.Core;
using Transformalize.Rhino.Etl.Core.Operations;

namespace Transformalize.Operations {
    public class TransformOperation : AbstractOperation {
        private readonly Entity _entity;

        public TransformOperation(Entity entity) {
            _entity = entity;
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            var fields = new FieldSqlWriter(_entity.All).ExpandXml().Input().HasDefaultOrTransform().Context();
            foreach (var row in rows) {
                foreach (var fieldKey in fields.Keys) {
                    if (row[fieldKey] == null) continue;

                    var field = fields[fieldKey];
                    if (field.Transforms == null) continue;

                    field.StringBuilder.Clear();
                    field.StringBuilder.Append(row[fieldKey]);
                    foreach (var transformer in field.Transforms) {
                        transformer.Transform(field.StringBuilder);
                    }

                    row[fieldKey] = field.StringBuilder.ToString();
                }
                yield return row;
            }
        }
    }
}