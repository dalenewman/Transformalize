using System.Data;
using System.Linq;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Main;
using Transformalize.Main.Providers;

namespace Transformalize.Operations
{
    public class EntityInputKeysExtractAllForDelete : InputCommandOperation {
        private readonly Entity _entity;
        private readonly AbstractConnection _connection;
        private readonly string[] _fields;

        public EntityInputKeysExtractAllForDelete(Entity entity, AbstractConnection connection)
            : base(connection) {
            _entity = entity;
            _connection = connection;
            _fields = new FieldSqlWriter(entity.PrimaryKey).Input().Alias(connection.Provider).Keys().ToArray();
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
            cmd.CommandText = _connection.KeyAllQuery(_entity);
        }

    }
}