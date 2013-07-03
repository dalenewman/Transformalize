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
                foreach (var pair in register) {
                    if (!_process.KeyRegister.ContainsKey(pair.Key)) {
                        _process.KeyRegister.Add(pair.Key, new HashSet<object>());
                    }
                    _process.KeyRegister[pair.Key].Add(row[pair.Key]);
                }
                yield return row;
            }
            
        }
    }
}