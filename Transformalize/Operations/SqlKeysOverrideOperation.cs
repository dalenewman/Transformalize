using System.Data;
using System.Linq;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Main;
using Transformalize.Main.Providers;

namespace Transformalize.Operations
{
    public class SqlKeysOverrideOperation : InputCommandOperation {

        private readonly Entity _entity;
        private readonly string[] _fields;
        private readonly int _length;

        public SqlKeysOverrideOperation(Entity entity, AbstractConnection connection)
            : base(connection) {
            _entity = entity;
            _fields = entity.PrimaryKey.Aliases().ToArray();
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
            cmd.CommandText = _entity.SqlKeysOverride;
            cmd.CommandTimeout = 0;
            cmd.CommandType = CommandType.Text;
        }
    }
}