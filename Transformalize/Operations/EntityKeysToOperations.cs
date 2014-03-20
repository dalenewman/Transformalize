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

using System;
using System.Collections.Generic;
using System.Linq;
using Transformalize.Extensions;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Main;
using Transformalize.Main.Providers;

namespace Transformalize.Operations {
    public class EntityKeysToOperations : AbstractOperation {
        private readonly Entity _entity;
        private readonly AbstractConnection _connection;
        private readonly Field[] _key;
        private readonly string _operationColumn;
        private readonly IList<Row> _keys = new List<Row>();

        public EntityKeysToOperations(Entity entity, AbstractConnection connection, string operationColumn = "operation") {
            _entity = entity;
            _connection = connection;
            _operationColumn = operationColumn;
            _key = new FieldSqlWriter(_entity.PrimaryKey).Input().ToArray();
            _keys = new List<Row>(_entity.InputKeys.Select(r => r.Clone()));
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            var fields = new FieldSqlWriter(_entity.Fields).Input().ToArray();

            if (_keys.Count > 0 && _keys.Count < _connection.BatchSize) {
                yield return GetOperationRow(_keys, fields);
            } else {
                foreach (var batch in _keys.Partition(_connection.BatchSize)) {
                    yield return GetOperationRow(batch, fields);
                }
            }
        }

        private Row GetOperationRow(IEnumerable<Row> batch, Field[] fields) {
            var sql = SelectByKeys(batch);
            var row = new Row();
            row[_operationColumn] = new EntityDataExtract(fields, sql, _connection);
            return row;
        }

        public string SelectByKeys(IEnumerable<Row> rows) {
            var tableName = _connection.Provider.Supports.TableVariable ? "@KEYS" : "keys_" + _entity.Name;
            var noCount = _connection.Provider.Supports.NoCount ? "SET NOCOUNT ON;\r\n" : string.Empty;
            var sql = noCount +
                _connection.TableQueryWriter.WriteTemporary(tableName, _key, _connection.Provider, false) +
                SqlTemplates.BatchInsertValues(50, tableName, _key, rows, _connection) + Environment.NewLine +
                SqlTemplates.Select(_entity.Fields, _entity.Name, tableName, _connection.Provider) +
                (_connection.Provider.Supports.TableVariable ? string.Empty : string.Format("DROP TABLE {0};", tableName));

            Trace(sql);

            return sql;
        }
    }
}