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
        private readonly int _length;

        public EntityInputKeysExtractAllForDelete(Entity entity, AbstractConnection connection)
            : base(connection) {
            _entity = entity;
            _connection = connection;
            _fields = _entity.PrimaryKey.ToEnumerable().Where(f => f.Input).Select(f => f.Alias).ToArray();
            _length = _fields.Length;
        }

        protected override Row CreateRowFromReader(IDataReader reader) {
            var row = new Row();
            for (var i = 0; i < _length; i++) {
                row[_fields[i]] = reader.GetValue(i);
            }
            return row;
        }

        protected override void PrepareCommand(IDbCommand cmd) {
            cmd.CommandTimeout = 0;
            cmd.CommandText = _connection.KeyAllQuery(_entity);
        }

    }
}