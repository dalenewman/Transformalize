using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using Transformalize.Extensions;
using Transformalize.Libs.Dapper;
using Transformalize.Main;
using Transformalize.Main.Providers;

namespace Transformalize.Libs.Rhino.Etl.Operations {

    public class DapperBulkUpdateOperation : AbstractOperation {
        private readonly AbstractConnection _connection;
        private readonly Fields _fields;
        private readonly string _sql;
        private readonly int _tflBatchId;

        public DapperBulkUpdateOperation(AbstractConnection connection, Entity entity) {
            _connection = connection;
            _tflBatchId = entity.TflBatchId;
            _fields = entity.OutputFields();
            var writer = new FieldSqlWriter(_fields);
            var sets = writer.Alias(_connection.L, _connection.R).SetParam().Write(", ", false);

            _sql = string.Format(@"UPDATE [{0}] SET {1}, TflBatchId = @TflBatchId WHERE TflKey = @TflKey;", entity.OutputName(), sets);
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            using (var cn = _connection.GetConnection()) {
                cn.Open();
                var transaction = cn.BeginTransaction();
                try {
                    foreach (var group in rows.Partition(_connection.BatchSize)) {
                        cn.Execute(_sql, group.Select(ToExpandoObject), transaction, 0);
                    }
                    transaction.Commit();
                } catch (Exception ex) {
                    transaction.Rollback();
                    throw new TransformalizeException("The bulk update operation failed. {0}", ex.Message);
                }
            }
            yield break;
        }

        public IEnumerable<KeyValuePair<string, object>> Convert(Row row) {
            return _fields.Select(field => new KeyValuePair<string, object>(field.Identifier, row[field.Alias])).ToArray();
        }

        public ExpandoObject ToExpandoObject(Row row) {
            var parameters = new ExpandoObject();
            foreach (var field in _fields) {
                ((IDictionary<string, object>)parameters).Add(field.Identifier, row[field.Alias]);
            }
            ((IDictionary<string, object>)parameters)["TflKey"] = row["TflKey"];
            ((IDictionary<string, object>)parameters)["TflBatchId"] = _tflBatchId;
            return parameters;
        }


    }
}