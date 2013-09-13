using System.Data;
using Transformalize.Main;
using Transformalize.Main.Providers;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;

namespace Transformalize.Operations
{
    public class EntityDataExtract : InputCommandOperation
    {
        private readonly Entity _entity;
        private readonly string[] _fields;
        private readonly string _sql;


        public EntityDataExtract(Entity entity, string[] fields, string sql, AbstractConnection connection) : base(connection)
        {
            _entity = entity;
            _fields = fields;
            _sql = sql;

            UseTransaction = false;
        }

        protected override Row CreateRowFromReader(IDataReader reader)
        {
            var row = new Row();
            foreach (string field in _fields)
            {
                row[field] = reader[field];
            }
            return row;
        }

        protected override void PrepareCommand(IDbCommand cmd)
        {
            cmd.CommandText = _sql;
            cmd.CommandTimeout = 0;
            cmd.CommandType = CommandType.Text;
        }
    }
}