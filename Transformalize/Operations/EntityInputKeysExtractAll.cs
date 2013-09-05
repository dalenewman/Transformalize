using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Transformalize.Core.Entity_;
using Transformalize.Core.Field_;
using Transformalize.Core.Fields_;
using Transformalize.Libs.Rhino.Etl.Core;
using Transformalize.Libs.Rhino.Etl.Core.Operations;

namespace Transformalize.Operations
{
    public class EntityInputKeysExtractAll : InputCommandOperation
    {
        private readonly Entity _entity;
        private readonly IEnumerable<string> _fields;

        public EntityInputKeysExtractAll(Entity entity)
            : base(entity.InputConnection.ConnectionString)
        {
            _entity = entity;
            _entity.End = _entity.EntityVersionReader.GetEndVersion();
            _fields = new FieldSqlWriter(entity.PrimaryKey).Alias().Keys();
            if (!_entity.EntityVersionReader.HasRows)
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
            const string sqlPattern = "SELECT {0}\r\nFROM [{1}].[{2}] WITH (NOLOCK)\r\nWHERE [{3}] <= @End\r\nORDER BY {4};";
            return string.Format(sqlPattern, string.Join(", ", _entity.SelectKeys()), _entity.Schema, _entity.Name, _entity.Version.Name, string.Join(", ", _entity.OrderByKeys()));
        }

        protected override void PrepareCommand(IDbCommand cmd)
        {
            cmd.CommandTimeout = 0;
            cmd.CommandText = PrepareSql();
            cmd.Parameters.Add(new SqlParameter("@End", _entity.End));
        }
    }
}