using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Transformalize.Core.Entity_;
using Transformalize.Core.Field_;
using Transformalize.Extensions;
using Transformalize.Libs.Rhino.Etl.Core;
using Transformalize.Libs.Rhino.Etl.Core.Operations;
using Transformalize.Providers;
using Transformalize.Providers.SqlServer;

namespace Transformalize.Operations
{
    public class ExtractDataDifferently : AbstractOperation
    {
        private const string KEYS_TABLE_VARIABLE = "@KEYS";
        private readonly Entity _entity;
        private readonly string[] _fields;
        private bool _debugFirstQuery;
        private readonly Field[] _key;

        public ExtractDataDifferently(Entity entity)
        {
            _entity = entity;
            _fields = new FieldSqlWriter(_entity.All).ExpandXml().Input().Keys().ToArray();
            _key = new FieldSqlWriter(_entity.PrimaryKey).ToArray();
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
        {
            var commands = rows.Partition(_entity.InputConnection.BatchSize).Select(SelectByKeys).ToList().AsParallel();

            var newRows = new List<Row>();
            using (var connection = new SqlConnection(_entity.InputConnection.ConnectionString))
            {
                connection.Open();
                foreach (var sqlCommand in commands)
                {
                    var command = new SqlCommand(sqlCommand, connection);

                    if (_entity.Name == "WorkOrderExt")
                    {
                        Info(command.CommandText);
                    }
                    using (var reader = command.ExecuteReader(CommandBehavior.SequentialAccess))
                    {
                        while (reader.Read())
                        {
                            var row = new Row();
                            foreach (var field in _fields)
                            {
                                row[field] = reader[field];
                            }
                            newRows.Add(row);
                        }
                    }
                    
                    foreach (var row in newRows)
                    {
                        yield return row;
                    }
                    newRows.Clear();
                }
            }

        }

        public string SelectByKeys(IEnumerable<Row> rows)
        {
            var sql = "SET NOCOUNT ON;\r\n" +
                      SqlTemplates.CreateTableVariable(KEYS_TABLE_VARIABLE, _key, false) +
                      SqlTemplates.BatchInsertValues(50, KEYS_TABLE_VARIABLE, _key, rows, ((SqlServerConnection)_entity.InputConnection).InsertMultipleValues()) + Environment.NewLine +
                      SqlTemplates.Select(_entity.All, _entity.Name, KEYS_TABLE_VARIABLE);

            if (_debugFirstQuery)
                Trace(sql);
            else
            {
                Debug(sql);
                _debugFirstQuery = true;
            }

            return sql;
        }

    }
}