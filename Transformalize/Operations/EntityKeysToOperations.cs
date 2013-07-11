using System;
using System.Collections.Generic;
using System.Linq;
using Transformalize.Model;
using Transformalize.Rhino.Etl.Core;
using Transformalize.Rhino.Etl.Core.ConventionOperations;
using Transformalize.Rhino.Etl.Core.Operations;

namespace Transformalize.Operations {

    public class EntityKeysToOperations : AbstractOperation {
        private readonly Entity _entity;
        private readonly string _operationColumn;
        private const string KEYS_TABLE_VARIABLE = "@KEYS";

        public EntityKeysToOperations(Entity entity, string operationColumn = "operation") {
            _entity = entity;
            _operationColumn = operationColumn;
        }
        
        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {

            return rows.Partition(_entity.InputConnection.InputBatchSize).Select(batch => new Row {{
                _operationColumn,
                new ConventionInputCommandOperation(_entity.InputConnection.ConnectionString) {
                    Command = SelectByKeys(batch)
                }
            }});
        }

        public string SelectByKeys(IEnumerable<Row> rows) {
            var context = new FieldSqlWriter(_entity.PrimaryKey).Context();
            var sql = "SET NOCOUNT ON;\r\n" +
                      SqlTemplates.CreateTableVariable(KEYS_TABLE_VARIABLE, context, false) +
                      SqlTemplates.BatchInsertValues(50, KEYS_TABLE_VARIABLE, context, rows, _entity.InputConnection.Year) + Environment.NewLine +
                      SqlTemplates.Select(_entity.All, _entity.Name, KEYS_TABLE_VARIABLE);

            Trace(sql);

            return sql;
        }
    }
}