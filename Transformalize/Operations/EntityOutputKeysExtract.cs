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

using System.Data;
using Transformalize.Model;
using Transformalize.Rhino.Etl.Core;
using Transformalize.Rhino.Etl.Core.Operations;

namespace Transformalize.Operations
{
    public class EntityOutputKeysExtract : InputCommandOperation {

        const string SQL_PATTERN = @"SELECT {0}, TflKey FROM [{1}].[{2}] WITH (NOLOCK) ORDER BY {3};";
        private readonly Entity _entity;
        private readonly string _sql;

        public EntityOutputKeysExtract(Entity entity)
            : base(entity.OutputConnection.ConnectionString) {
            _entity = entity;
            _sql = PrepareSql();
            }

        protected override Row CreateRowFromReader(IDataReader reader) {
            return Row.FromReader(reader);
        }

        protected override void PrepareCommand(IDbCommand cmd) {
            cmd.CommandTimeout = 0;
            cmd.CommandText = _sql;
        }

        private string PrepareSql() {
            var sqlWriter = new FieldSqlWriter(_entity.PrimaryKey).Alias();
            var selectKeys = sqlWriter.Write(", ", false);
            var orderByKeys = sqlWriter.Asc().Write();
            return string.Format(SQL_PATTERN, selectKeys, _entity.Schema, _entity.OutputName(), orderByKeys);
        }
    }
}