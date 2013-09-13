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

using System;
using System.Collections.Generic;
using System.Linq;
using Transformalize.Main;
using Transformalize.Main.Providers;
using Transformalize.Extensions;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;

namespace Transformalize.Operations
{
    public class EntityKeysToOperations : AbstractOperation
    {
        private readonly Entity _entity;
        private readonly Field[] _key;
        private readonly string _operationColumn;
        private readonly AbstractProvider _provider;

        public EntityKeysToOperations(Entity entity, string operationColumn = "operation")
        {
            _entity = entity;
            _provider = _entity.InputConnection.Provider;
            _operationColumn = operationColumn;
            _key = new FieldSqlWriter(_entity.PrimaryKey).ToArray();
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
        {
            string[] fields = new FieldSqlWriter(_entity.All).ExpandXml().Input().Keys().ToArray();

            int count = 0;
            foreach (var batch in _entity.InputKeys.Partition(_entity.InputConnection.BatchSize))
            {
                string sql = SelectByKeys(batch);
                var row = new Row();
                row[_operationColumn] = new EntityDataExtract(_entity, fields, sql, _entity.InputConnection);
                count++;
                yield return row;
            }
        }

        public string SelectByKeys(IEnumerable<Row> rows)
        {
            string tableName = _provider.Supports.TableVariable ? "@KEYS" : "KEYS_" + _entity.Name;
            string noCount = _provider.Supports.NoCount ? "SET NOCOUNT ON;\r\n" : string.Empty;
            string sql = noCount +
                         _entity.InputConnection.TableQueryWriter.WriteTemporary(tableName, _key, _provider, false) +
                         SqlTemplates.BatchInsertValues(50, tableName, _key, rows, _entity.InputConnection) + Environment.NewLine +
                         SqlTemplates.Select(_entity.All, _entity.Name, tableName, _provider);

            Trace(sql);

            return sql;
        }
    }
}