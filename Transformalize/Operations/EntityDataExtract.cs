using System.Collections.Generic;
using System.Data;
using System.Linq;
using Transformalize.Core;
using Transformalize.Core.Entity_;
using Transformalize.Core.Field_;
using Transformalize.Libs.Rhino.Etl.Core;
using Transformalize.Libs.Rhino.Etl.Core.Operations;
using Transformalize.Providers;

namespace Transformalize.Operations
{
    public class EntityDataExtract : InputCommandOperation
    {
        private readonly string[] _fields;
        private readonly Entity _entity;
        private readonly string _sql;


        public EntityDataExtract(Entity entity, string[] fields, string sql, AbstractConnection connection ) : base(connection)
        {
            _entity = entity;
            _fields = fields;
            _sql = sql;

            UseTransaction = false;
        }

        protected override Row CreateRowFromReader(IDataReader reader)
        {
            var row = new Row();
            foreach (var field in _fields)
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