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
        private readonly Field[] _key;
        private readonly string _operationColumn;
        private readonly AbstractProvider _provider;

        public EntityKeysToOperations(Entity entity, string operationColumn = "operation") {
            _entity = entity;
            _provider = _entity.InputConnection.Provider;
            _operationColumn = operationColumn;
            _key = new FieldSqlWriter(_entity.PrimaryKey).Input().ToArray();
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            var fields = new FieldSqlWriter(_entity.Fields).Input().Keys().ToArray();

            var operationRows = new List<Row>();

            if (_entity.InputKeys.Count > 0 && _entity.InputKeys.Count < _entity.InputConnection.BatchSize) {
                operationRows.Add(GetOperationRow(_entity.InputKeys, fields));
            } else {
                operationRows.AddRange(_entity.InputKeys.Partition(_entity.InputConnection.BatchSize).Select(batch => GetOperationRow(batch, fields)));
            }

            return operationRows;
        }

        private Row GetOperationRow(IEnumerable<Row> batch, string[] fields) {
            var sql = SelectByKeys(batch);
            var row = new Row();
            row[_operationColumn] = new EntityDataExtract(fields, sql, _entity.InputConnection);
            return row;
        }

        public string SelectByKeys(IEnumerable<Row> rows) {
            var tableName = _provider.Supports.TableVariable ? "@KEYS" : "KEYS_" + _entity.Name;
            var noCount = _provider.Supports.NoCount ? "SET NOCOUNT ON;\r\n" : string.Empty;
            var sql = noCount +
                      _entity.InputConnection.TableQueryWriter.WriteTemporary(tableName, _key, _provider, false) +
                      SqlTemplates.BatchInsertValues(50, tableName, _key, rows, _entity.InputConnection) + Environment.NewLine +
                      SqlTemplates.Select(_entity.Fields, _entity.Name, tableName, _provider);

            Trace(sql);

            return sql;
        }
    }
}