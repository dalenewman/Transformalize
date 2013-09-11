using System;
using System.Data;
using System.Linq;
using Transformalize.Core.Entity_;
using Transformalize.Core.Field_;
using Transformalize.Core.Process_;
using Transformalize.Libs.Rhino.Etl.Core;
using Transformalize.Libs.Rhino.Etl.Core.Operations;
using Transformalize.Providers;

namespace Transformalize.Operations
{
    public class EntityInputKeysExtractAll : InputCommandOperation
    {
        private readonly Entity _entity;
        private readonly string[] _fields;

        public EntityInputKeysExtractAll(Entity entity)
            : base(entity.InputConnection)
        {
            _entity = entity;

            var connection = _entity.InputConnection;

            if (entity.Version != null)
            {
                connection.LoadEndVersion(_entity);
                if (!_entity.HasRows)
                {
                    Debug("No data detected in {0}.", _entity.Alias);
                }
            }

            _fields = new FieldSqlWriter(entity.PrimaryKey).Alias(connection.Provider).Keys().ToArray();
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
            cmd.CommandTimeout = 0;
            cmd.CommandText = _entity.KeysQuery();
            AddParameter(cmd, "@End", _entity.End);
        }
    }
}