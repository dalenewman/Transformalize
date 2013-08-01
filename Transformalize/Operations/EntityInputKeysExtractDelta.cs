/*
Transformalize - Replicate, Transform, and Denormalize Your Data...
Copyright (C) 2013 Dale Newman

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Transformalize.Libs.Rhino.Etl.Core;
using Transformalize.Libs.Rhino.Etl.Core.Operations;
using Transformalize.Model;

namespace Transformalize.Operations {
    public class EntityInputKeysExtractDelta : InputCommandOperation {

        private readonly Entity _entity;
        private readonly List<string> _selectKeys = new List<string>();
        private readonly List<string> _orderByKeys = new List<string>();

        public EntityInputKeysExtractDelta(Entity entity)
            : base(entity.InputConnection.ConnectionString) {

            _entity = entity;
            _entity.Begin = _entity.EntityVersionReader.GetBeginVersion();
            _entity.End = _entity.EntityVersionReader.GetEndVersion();
            if (!_entity.EntityVersionReader.HasRows) {
                Debug("{0} | No data detected in {1}.", _entity.ProcessName, _entity.Name);
            }

            if (!_entity.EntityVersionReader.IsRange) return;

            if (_entity.EntityVersionReader.BeginAndEndAreEqual()) {
                Debug("{0} | No changes detected in {1}.", _entity.ProcessName, _entity.Name);
            }
        }

        protected override Row CreateRowFromReader(IDataReader reader) {
            return Row.FromReader(reader);
        }

        protected override void PrepareCommand(IDbCommand cmd) {
            cmd.CommandTimeout = 0;
            cmd.CommandText = PrepareSql(_entity.EntityVersionReader.IsRange);

            if (_entity.EntityVersionReader.IsRange)
                cmd.Parameters.Add(new SqlParameter("@Begin", _entity.Begin));
            cmd.Parameters.Add(new SqlParameter("@End", _entity.End));
        }

        public string PrepareSql(bool isRange) {
            const string sqlPattern = "SELECT {0}\r\nFROM [{1}].[{2}] WITH (NOLOCK)\r\nWHERE {3}\r\nORDER BY {4};";

            var criteria = string.Format(isRange ? "[{0}] BETWEEN @Begin AND @End" : "[{0}] <= @End", _entity.Version.Name);

            foreach (var pair in _entity.PrimaryKey) {
                _selectKeys.Add(pair.Value.Alias.Equals(pair.Value.Name) ? string.Concat("[", pair.Value.Name, "]") : string.Format("{0} = [{1}]", pair.Value.Alias, pair.Value.Name));
                _orderByKeys.Add(string.Concat("[", pair.Value.Name, "]"));
            }

            return string.Format(sqlPattern, string.Join(", ", _selectKeys), _entity.Schema, _entity.Name, criteria, string.Join(", ", _orderByKeys));
        }

        public bool NeedsToRun() {
            return _entity.NeedsUpdate();
        }

    }
}