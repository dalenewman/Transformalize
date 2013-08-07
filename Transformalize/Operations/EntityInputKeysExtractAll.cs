using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Transformalize.Core.Entity_;
using Transformalize.Libs.Rhino.Etl.Core;
using Transformalize.Libs.Rhino.Etl.Core.Operations;

namespace Transformalize.Operations
{
    public class EntityInputKeysExtractAll : InputCommandOperation
    {
        private readonly Entity _entity;
        private readonly List<string> _selectKeys = new List<string>(); 
        private readonly List<string> _orderByKeys = new List<string>();

        public EntityInputKeysExtractAll(Entity entity)
            : base(entity.InputConnection.ConnectionString)
        {
            _entity = entity;
            _entity.End = _entity.EntityVersionReader.GetEndVersion();
            if (!_entity.EntityVersionReader.HasRows)
            {
                Debug("{0} | No data detected in {1}.", _entity.ProcessName, _entity.Name);
            }
        }

        protected override Row CreateRowFromReader(IDataReader reader)
        {
            return Row.FromReader(reader);
        }

        protected string PrepareSql()
        {
            const string sqlPattern = "SELECT {0}\r\nFROM [{1}].[{2}] WITH (NOLOCK)\r\nWHERE [{3}] <= @End\r\nORDER BY {4};";

            foreach (var pair in _entity.PrimaryKey)
            {
                _selectKeys.Add(pair.Value.Alias.Equals(pair.Value.Name) ? string.Concat("[", pair.Value.Name, "]") : string.Format("{0} = [{1}]", pair.Value.Alias, pair.Value.Name));
                _orderByKeys.Add(string.Concat("[", pair.Value.Name, "]"));
            }

            return string.Format(sqlPattern, string.Join(", ", _selectKeys), _entity.Schema, _entity.Name, _entity.Version.Name, string.Join(", ", _orderByKeys));
            
        }

        protected override void PrepareCommand(IDbCommand cmd)
        {
            cmd.CommandTimeout = 0;
            cmd.CommandText = PrepareSql();
            cmd.Parameters.Add(new SqlParameter("@End", _entity.End));
        }
    }
}