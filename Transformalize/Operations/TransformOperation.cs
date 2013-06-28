using System.Collections.Generic;
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
                row["TflId"] = _entity.TflId;
                foreach (var key in fields.Keys) {
                    var field = fields[key];
                    if (row[key] == null) {
                        row[key] = field.Default;
                        continue;
                    }

                    if (field.Transforms == null) continue;

                    if (field.UseStringBuilder) {
                        field.StringBuilder.Clear();
                        field.StringBuilder.Append(row[key]);
                        foreach (var transformer in field.Transforms) {
                            transformer.Transform(field.StringBuilder);
                        }
                        row[key] = field.StringBuilder.ToString();
                    } else {
                        foreach (var transformer in field.Transforms) {
                            row[key] = transformer.Transform(row[key]);
                        }
                    }
                }
                yield return row;
            }
        }
    }
}