#region License

// /*
// Transformalize - Replicate, Transform, and Denormalize Your Data...
// Copyright (C) 2013 Dale Newman
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// */

#endregion

using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;

namespace Transformalize.Main.Providers.SqlServer {

    public class SqlServerEntityKeysExtractFromOutput : InputCommandOperation {

        private readonly AbstractConnection _connection;
        private readonly Entity _entity;
        private readonly List<string> _fields;
        private readonly Fields _key;

        public SqlServerEntityKeysExtractFromOutput(AbstractConnection connection, Entity entity)
            : base(connection) {
            _connection = connection;
            EntityName = entity.Name;
            _entity = entity;
            _fields = new List<string>(new FieldSqlWriter(entity.PrimaryKey).AddDeleted(entity).Alias(connection.L, connection.R).Keys()) { "TflKey" };
            _key = _entity.PrimaryKey;
        }

        protected override Row CreateRowFromReader(IDataReader reader) {
            var row = new Row();
            foreach (var field in _fields) {
                row[field] = reader[field];
            }
            return row;
        }

        protected override void PrepareCommand(IDbCommand cmd) {
            cmd.CommandTimeout = 0;
            cmd.CommandText = 
                _entity.CanDetectChanges(Connection.IsDatabase) && (_entity.HasSqlKeysOverride() || !_entity.HasSqlOverride()) ?
                PrepareSqlWithInputKeys() :
                PrepareSql();
            Debug("SQL:\r\n{0}", cmd.CommandText);
        }

        private string PrepareSql() {
            const string sqlPattern = @"
                SELECT e.{0}, e.TflKey{1}
                FROM {2} e WITH (NOLOCK);
            ";

            var selectKeys = new FieldSqlWriter(_entity.PrimaryKey).AddDeleted(_entity).Alias(_connection.L, _connection.R).Write(", e.", false);
            return string.Format(sqlPattern, selectKeys, PrepareVersion(), _connection.Enclose(_entity.OutputName()));
        }

        private string PrepareVersion() {
            if (_entity.Version == null || VersionIsPrimaryKey() || _fields.Contains(_entity.Version.Alias))
                return string.Empty;

            _fields.Add(_entity.Version.Alias);
            return ", e." + _connection.Enclose(_entity.Version.Alias);
        }

        private bool VersionIsPrimaryKey() {
            var version = _entity.Version.Alias;
            return _entity.PrimaryKey.Count == 1 && version.Equals(_entity.PrimaryKey.First().Alias);
        }

        private string PrepareSqlWithInputKeys() {

            const string sqlPattern = @"
                {0}

                SELECT e.{1}, e.TflKey{2}
                FROM {3} e WITH (NOLOCK)
                INNER JOIN @KEYS k ON ({4});
            ";

            var builder = new StringBuilder();
            builder.AppendLine(_connection.WriteTemporaryTable("@KEYS", _key.WithInput()));
            builder.AppendLine(SqlTemplates.BatchInsertValues(50, "@KEYS", _key.WithInput(), _entity.InputKeys, _connection));

            var selectKeys = new FieldSqlWriter(_entity.PrimaryKey).AddDeleted(_entity).Alias(_connection.L, _connection.R).Write(", e.", false);
            var joinKeys = new FieldSqlWriter(_entity.PrimaryKey).Input().Alias(_connection.L, _connection.R).Set("e", "k").Write(" AND ");
            return string.Format(sqlPattern, builder, selectKeys, PrepareVersion(), _connection.Enclose(_entity.OutputName()), joinKeys);
        }
    }
}