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
using System.Text;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Main;
using Transformalize.Main.Providers;

namespace Transformalize.Operations {
    public class EntityOutputKeysExtractAll : InputCommandOperation {
        private readonly Entity _entity;
        private readonly List<string> _fields;
        private readonly Field[] _key;

        public EntityOutputKeysExtractAll(Entity entity)
            : base(entity.OutputConnection) {
            _entity = entity;
            _fields = new List<string>(new FieldSqlWriter(entity.PrimaryKey).Alias(entity.OutputConnection.Provider).Keys()) { "TflKey" };
            _key = new FieldSqlWriter(entity.PrimaryKey).ToArray();

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
            cmd.CommandText = PrepareSql();
            Debug("SQL:\r\n{0}", cmd.CommandText);
        }

        private string PrepareSql() {
            var connection = _entity.OutputConnection;
            var provider = connection.Provider;
            
            var sqlPattern = "SELECT {0}, TflKey FROM {1}";
            if (provider.Supports.NoLock) {
                sqlPattern += " WITH (NOLOCK);";
            } else {
                sqlPattern = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " + sqlPattern + "; COMMIT;";
            }

            var selectKeys = new FieldSqlWriter(_entity.PrimaryKey).Alias(provider).Write(", ", false);
            return string.Format(sqlPattern, selectKeys, provider.Enclose(_entity.OutputName()));
        }

    }

    public class EntityOutputKeysExtract : InputCommandOperation {
        private readonly Entity _entity;
        private readonly List<string> _fields;
        private readonly Field[] _key;

        public EntityOutputKeysExtract(Entity entity)
            : base(entity.OutputConnection) {
            _entity = entity;
            _fields = new List<string>(new FieldSqlWriter(entity.PrimaryKey).Alias(entity.OutputConnection.Provider).Keys()) { "TflKey" };
            _key = new FieldSqlWriter(entity.PrimaryKey).ToArray();
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
            cmd.CommandText = PrepareSql();
            Debug("SQL:\r\n{0}", cmd.CommandText);
        }

        private string PrepareSql() {
            var connection = _entity.OutputConnection;
            var provider = connection.Provider;
            const string sqlPattern = @"
{0}

SELECT e.{1}, e.TflKey{2}
FROM {3} e WITH (NOLOCK)
INNER JOIN @KEYS k ON ({4});
";

            var builder = new StringBuilder();
            builder.AppendLine(_entity.OutputConnection.WriteTemporaryTable("@KEYS", _key));
            builder.AppendLine(SqlTemplates.BatchInsertValues(50, "@KEYS", _key, _entity.InputKeys, _entity.OutputConnection));

            var rowVersion = string.Empty;
            if (_entity.CanDetectChanges()) {
                _fields.Add(_entity.Version.Alias);
                rowVersion = ", e." + provider.Enclose(_entity.Version.Alias);
            }

            var selectKeys = new FieldSqlWriter(_entity.PrimaryKey).Alias(provider).Write(", e.", false);
            var joinKeys = new FieldSqlWriter(_entity.PrimaryKey).Alias(provider).Set("e", "k").Write(" AND ");
            return string.Format(sqlPattern, builder, selectKeys, rowVersion, provider.Enclose(_entity.OutputName()), joinKeys);
        }
    }
}