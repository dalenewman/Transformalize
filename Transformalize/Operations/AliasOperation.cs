using System.Collections.Generic;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Main;

namespace Transformalize.Operations {
    public class AliasOperation : AbstractOperation {
        private readonly Field[] _fields;

        public AliasOperation(Entity entity) {
            _fields = entity.Fields.WithAlias().ToArray();
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                foreach (var field in _fields) {
                    row[field.Alias] = row[field.Name];
                }
                yield return row;
            }
        }
    }
}