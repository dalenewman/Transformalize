using System.Data;
using System.Linq;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Main;

namespace Transformalize.Operations
{
    public class EntityInputKeysExtractAllForDelete : InputCommandOperation {
        private readonly Entity _entity;
        private readonly string[] _fields;

        public EntityInputKeysExtractAllForDelete(Entity entity)
            : base(entity.InputConnection) {
            _entity = entity;
            _fields = new FieldSqlWriter(entity.PrimaryKey).Input().Alias(_entity.InputConnection.Provider).Keys().ToArray();
            }

        protected override Row CreateRowFromReader(IDataReader reader) {
            var row = new Row();
            foreach (var field in _fields) {
                row[field] = reader[field];
            }
            return row;
        }

        protected override void PrepareCommand(IDbCommand cmd) {
            cmd.CommandTimeout = 0;
            cmd.CommandText = _entity.KeysAllQuery();
        }

    }
}