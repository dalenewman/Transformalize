using System.Data;
using Transformalize.Core.Entity_;
using Transformalize.Libs.Rhino.Etl.Core;
using Transformalize.Libs.Rhino.Etl.Core.Operations;

namespace Transformalize.Operations
{
    public class EntityInputKeysExtractTop : InputCommandOperation
    {
        private readonly Entity _entity;
        private readonly int _top;

        public EntityInputKeysExtractTop(Entity entity, int top)
            : base(entity.InputConnection.ConnectionString)
        {
            _entity = entity;
            _top = top;
        }

        protected override Row CreateRowFromReader(IDataReader reader)
        {
            return Row.FromReader(reader);
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
        }
    }
}