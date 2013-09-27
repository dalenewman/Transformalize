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

namespace Transformalize.Operations {
    public class EntityInputKeysExtractAll : InputCommandOperation {
        private readonly Entity _entity;
        private readonly string[] _fields;

        //todo: you can't use input command operation to import file
        public EntityInputKeysExtractAll(Entity entity)
            : base(entity.InputConnection) {
            _entity = entity;

            var connection = _entity.InputConnection;
            if (entity.CanDetectChanges()) {
                connection.LoadEndVersion(_entity);
                if (!_entity.HasRows) {
                    Debug("No data detected in {0}.", _entity.Alias);
                }
            }

            _fields = new FieldSqlWriter(entity.PrimaryKey).Alias(connection.Provider).Keys().ToArray();
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
            cmd.CommandText = _entity.KeysQuery();
            AddParameter(cmd, "@End", _entity.End);
        }
    }
}