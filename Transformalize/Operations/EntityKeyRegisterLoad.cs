using System.Collections.Generic;
using Transformalize.Model;
using Transformalize.Rhino.Etl.Core;
using Transformalize.Rhino.Etl.Core.Operations;

namespace Transformalize.Operations {
    public class EntityKeyRegisterLoad : AbstractOperation {
        private readonly Process _process;
        private readonly Entity _entity;

        public EntityKeyRegisterLoad(Process process, Entity entity) {
            _process = process;
            _entity = entity;
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {

            var register = new FieldSqlWriter(_entity.All).FieldType(FieldType.ForeignKey).Context();

            foreach (var row in rows) {
                foreach (var fieldKey in register.Keys) {
                    if (!_process.KeyRegister.ContainsKey(fieldKey)) {
                        _process.KeyRegister.Add(fieldKey, new HashSet<object>());
                    }
                    _process.KeyRegister[fieldKey].Add(row[fieldKey]);
                }
            }
            yield break;
        }
    }
}