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

using System.Data.SqlClient;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;

namespace Transformalize.Main.Providers.SqlServer {
    public class SqlServerEntityBatchUpdate : SqlBatchOperation {
        private readonly AbstractConnection _connection;
        private readonly Entity _entity;

        public SqlServerEntityBatchUpdate(AbstractConnection connection, Entity entity)
            : base(connection) {
            _connection = connection;
            _entity = entity;
            BatchSize = connection.BatchSize;
            UseTransaction = true;
        }

        protected override void PrepareCommand(Row row, SqlCommand command) {
            
            var writer = new FieldSqlWriter(_entity.Fields, _entity.CalculatedFields).Output();
            var sets = writer.Alias(_connection.L, _connection.R).SetParam().Write(", ", false);

            command.CommandText = string.Format(@"
                UPDATE [{0}]
                SET {1}, TflBatchId = @TflBatchId
                WHERE TflKey = @TflKey;
            ", _entity.OutputName(), sets);

            foreach (var r in writer.ToArray()) {
                AddParameter(command, r.Alias, row[r.Alias]);
            }
            AddParameter(command, "TflKey", row["TflKey"]);
            AddParameter(command, "TflBatchId", _entity.TflBatchId);

            Debug(command.CommandText);
        }
    }
}