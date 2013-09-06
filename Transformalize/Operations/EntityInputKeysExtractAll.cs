using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Transformalize.Core.Entity_;
using Transformalize.Core.Field_;
using Transformalize.Core.Fields_;
using Transformalize.Core.Process_;
using Transformalize.Libs.Rhino.Etl.Core;
using Transformalize.Libs.Rhino.Etl.Core.Operations;

namespace Transformalize.Operations
{
    public class EntityInputKeysExtractAll : InputCommandOperation
    {
        private readonly Process _process;
        private readonly Entity _entity;
        private readonly IEnumerable<string> _fields;

        public EntityInputKeysExtractAll(Process process, Entity entity)
            : base(entity.InputConnection.ConnectionString)
        {
            _process = process;
            _entity = entity;
            _entity.InputConnection.LoadEndVersion(_entity);
            _fields = new FieldSqlWriter(entity.PrimaryKey).Alias().Keys();
            if (!_entity.HasRows)
            {
                Debug("No data detected in {0}.", _entity.Alias);
            }
        }

        protected override Row CreateRowFromReader(IDataReader reader)
        {
            var row = new Row();
            foreach (var field in _fields)
            {
                row.Add(field, reader[field]);
            }
            return row;
        }

        protected string PrepareSql()
        {
            const string sqlPattern = "SELECT {0}{1}\r\nFROM [{2}].[{3}] WITH (NOLOCK)\r\nWHERE [{4}] <= @End\r\nORDER BY {5};";
            var top = _process.Options.Top > 0 ? "TOP " + _process.Options.Top + " " : string.Empty;
            return string.Format(sqlPattern, top, string.Join(", ", _entity.SelectKeys()), _entity.Schema, _entity.Name, _entity.Version.Name, string.Join(", ", _entity.OrderByKeys()));
        }

        protected override void PrepareCommand(IDbCommand cmd)
        {
            cmd.CommandTimeout = 0;
            cmd.CommandText = PrepareSql();
            cmd.Parameters.Add(new SqlParameter("@End", _entity.End));
        }
    }
}