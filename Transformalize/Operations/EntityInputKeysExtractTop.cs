using System.Collections.Generic;
using System.Data;
using Transformalize.Core.Entity_;
using Transformalize.Core.Field_;
using Transformalize.Libs.NLog;
using Transformalize.Libs.Rhino.Etl.Core;
using Transformalize.Libs.Rhino.Etl.Core.Operations;

namespace Transformalize.Operations
{
    public class EntityInputKeysExtractTop : InputCommandOperation
    {
        private readonly Entity _entity;
        private readonly int _top;
        private readonly IEnumerable<string> _fields;
        private readonly Logger _log = LogManager.GetCurrentClassLogger();

        public EntityInputKeysExtractTop(Entity entity, int top)
            : base(entity.InputConnection.ConnectionString)
        {
            _entity = entity;
            _fields = new FieldSqlWriter(entity.PrimaryKey).Alias().Keys();
            _top = top;
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
            const string sqlPattern = "SELECT TOP {0} {1}\r\nFROM [{2}].[{3}] WITH (NOLOCK)\r\nORDER BY {4};";
            return string.Format(sqlPattern, _top, string.Join(", ", _entity.SelectKeys()), _entity.Schema, _entity.Name, string.Join(", ", _entity.OrderByKeys()));
        }

        protected override void PrepareCommand(IDbCommand cmd)
        {
            cmd.CommandTimeout = 0;
            cmd.CommandText = PrepareSql();
            cmd.CommandType = CommandType.Text;

            _log.Trace(cmd.CommandText);
        }
    }
}