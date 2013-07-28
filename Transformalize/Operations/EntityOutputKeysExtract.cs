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
using System.Text;
using Transformalize.Libs.Rhino.Etl.Core;
using Transformalize.Libs.Rhino.Etl.Core.Operations;
using Transformalize.Model;

namespace Transformalize.Operations {
    public class EntityOutputKeysExtract : InputCommandOperation {

        private readonly Entity _entity;

        public EntityOutputKeysExtract(Entity entity)
            : base(entity.OutputConnection.ConnectionString) {
            _entity = entity;
        }

        protected override Row CreateRowFromReader(IDataReader reader) {
            return Row.FromReader(reader);
        }

        protected override void PrepareCommand(IDbCommand cmd) {
            cmd.CommandTimeout = 0;
            cmd.CommandText = PrepareSql();
            Debug("\r\n{0}", cmd.CommandText);
        }

        private string PrepareSql() {
            const string sqlPattern = "{0}\r\nSELECT e.{1}, TflKey\r\nFROM [{2}].[{3}] e WITH (NOLOCK)\r\nINNER JOIN @KEYS k ON ({4})\r\nORDER BY {5};";

            var builder = new StringBuilder();
            builder.AppendLine(SqlTemplates.CreateTableVariable("@KEYS", _entity.PrimaryKey));
            builder.AppendLine(SqlTemplates.BatchInsertValues(50, "@KEYS", _entity.PrimaryKey, _entity.InputKeys, _entity.OutputConnection.InsertMultipleValues()));

            var selectKeys =  new FieldSqlWriter(_entity.PrimaryKey).Alias().Write(", e.", false);
            var joinKeys = new FieldSqlWriter(_entity.PrimaryKey).Alias().Set("e", "k").Write(" AND ");
            var orderByKeys = new FieldSqlWriter(_entity.PrimaryKey).Alias().Asc().Write();
            return string.Format(sqlPattern, builder, selectKeys, _entity.Schema, _entity.OutputName(), joinKeys, orderByKeys);
        }

    }
}