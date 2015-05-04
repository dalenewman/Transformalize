using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Transformalize.Extensions;
using Transformalize.Libs.Dapper;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Main;

namespace Transformalize.Operations {
    public class SqlResultsLoad : AbstractOperation {
        private readonly Process _process;
        private readonly string _sql;
        private readonly Fields _fields;

        public SqlResultsLoad(Process process) {
            _process = process;
            _fields = _process.CalculatedFields;
            var sets = new FieldSqlWriter(_fields).Alias(_process.OutputConnection.L, _process.OutputConnection.R).SetParam().Write();
            _sql = string.Format("UPDATE {0} SET {1} WHERE TflKey = @TflKey;", _process.MasterEntity.OutputName(), sets);
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            using (var cn = _process.OutputConnection.GetConnection()) {
                cn.Open();
                var transaction = cn.BeginTransaction();
                try {
                    foreach (var group in rows.Partition(_process.OutputConnection.BatchSize)) {
                        cn.Execute(_sql, group.Select(ToExpandoObject), transaction, 0);
                    }
                    transaction.Commit();
                } catch (Exception ex) {
                    transaction.Rollback();
                    throw new TransformalizeException(Logger, "Results batch update failed. {0}", ex.Message);
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
            ((IDictionary<string, object>)parameters).Add("TflKey", row["TflKey"]);
            return parameters;
        }
    }
}