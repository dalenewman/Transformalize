using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Transformalize.Core;
using Transformalize.Core.Entity_;
using Transformalize.Core.Field_;
using Transformalize.Core.Process_;
using Transformalize.Libs.Rhino.Etl.Core;
using Transformalize.Libs.Rhino.Etl.Core.Operations;

namespace Transformalize.Operations
{
    public class EntityInputKeysExtractAll : InputCommandOperation
    {
        private const StringComparison IC = StringComparison.OrdinalIgnoreCase;
        private readonly Process _process;
        private readonly Entity _entity;
        private readonly string[] _fields;

        public EntityInputKeysExtractAll(Process process, Entity entity)
            : base(entity.InputConnection)
        {
            _process = process;
            _entity = entity;
            _entity.InputConnection.LoadEndVersion(_entity);
            _fields = new FieldSqlWriter(entity.PrimaryKey).Alias().Keys().ToArray();
            
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
                row[field] = reader[field];
            }
            return row;
        }

        protected string PrepareSql()
        {
            const string sqlPattern = @"
                SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
                SELECT {0}{1}
                FROM {2}{3}
                WHERE {4} <= @End
                {5}
                {6}
            ";
            var top = _process.Options.Top > 0 && Connection.Compatibility.SupportsTop ? "TOP " + _process.Options.Top + " " : string.Empty;
            var limit = _process.Options.Top > 0 && Connection.Compatibility.SupportsLimit ? "LIMIT 0, " + _process.Options.Top + ";" : ";";
            var schema = _entity.Schema.Equals("dbo", IC) ? string.Empty : Connection.Provider.Enclose(_entity.Schema) + ".";
            var name = Connection.Provider.Enclose(_entity.Name);
            var version = Connection.Provider.Enclose(_entity.Version.Name);
            var commit = Connection.Compatibility.SupportsNoLock ? string.Empty : "\r\nCOMMIT;";
            return string.Format(sqlPattern, top, string.Join(", ", _entity.SelectKeys(Connection.Provider)), schema, name, version, limit, commit);
        }

        protected override void PrepareCommand(IDbCommand cmd)
        {
            cmd.CommandTimeout = 0;
            cmd.CommandText = PrepareSql();
            AddParameter(cmd, "@End", _entity.End);
        }
    }
}