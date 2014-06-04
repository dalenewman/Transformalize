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

using System.Data;
using System.Linq;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Main;
using Transformalize.Main.Providers;

namespace Transformalize.Operations {

    public class EntityInputKeysExtractAll : InputCommandOperation {

        private readonly Entity _entity;
        private readonly string[] _fields;
        private readonly int _length;
        private readonly string _sql;

        public EntityInputKeysExtractAll(Entity entity, AbstractConnection connection)
            : base(connection) {

            _entity = entity;
            _fields = _entity.PrimaryKey.WithInput().Aliases().ToArray();
            _length = _fields.Length;

            if (_entity.CanDetectChanges(connection.IsDatabase)) {
                connection.LoadEndVersion(_entity);
                if (!_entity.HasRows) {
                    Debug("No data detected in {0}.", _entity.Alias);
                }
            }

            if (connection.Schemas && entity.Schema.Equals(string.Empty)) {
                entity.Schema = connection.DefaultSchema;
            }

            _sql = _entity.CanDetectChanges(connection.IsDatabase)
                ? connection.KeyQuery(_entity)
                : connection.KeyAllQuery(_entity);
        }

        protected override Row CreateRowFromReader(IDataReader reader) {
            var row = new Row();
            for (var i = 0; i < _length; i++) {
                row[_fields[i]] = reader.GetValue(i);
            }
            return row;
        }

        protected override void PrepareCommand(IDbCommand cmd) {
            cmd.CommandTimeout = 0;
            cmd.CommandText = _sql;
            AddParameter(cmd, "@End", _entity.End);
        }
    }
}