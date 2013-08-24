using System.Collections.Generic;
using System.Data;
using Transformalize.Libs.Rhino.Etl.Core;
using Transformalize.Libs.Rhino.Etl.Core.Operations;

namespace Transformalize.Operations
{
    public class EntityDataExtract : InputCommandOperation
    {
        private readonly IEnumerable<string> _fields;
        private readonly string _sql;


        public EntityDataExtract(IEnumerable<string> fields, string sql, string connectionString ) : base(connectionString)
        {
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