using System.Collections.Generic;
using System.Data;
using Transformalize.Model;
using Transformalize.Rhino.Etl.Core;
using Transformalize.Rhino.Etl.Core.Operations;

namespace Transformalize.Operations {
    public class ParametersExtract : InputCommandOperation {
        private readonly string _sql;

        private string BuildSql(Process process, ICollection<int> tflId) {
            var fields = new FieldSqlWriter(process.Parameters).Alias().Write();
            var tflIdWhereClause = tflId.Count > 0 ? string.Format(" WHERE [TflId] IN ({0})", string.Join(", ", tflId)) : string.Empty;
            var sql = string.Format("SELECT [TflKey], {0} FROM {1}{2};", fields, process.Output, tflIdWhereClause);
            Debug("{0} | SQL:\r\n{1}", process.Name, sql);
            return sql;
        }

        public ParametersExtract(Process process, ICollection<int> tflId)
            : base(process.OutputConnection.ConnectionString) {
            UseTransaction = false;
            _sql = BuildSql(process, tflId);
        }

        protected override Row CreateRowFromReader(IDataReader reader) {
            return Row.FromReader(reader);
        }

        protected override void PrepareCommand(IDbCommand cmd) {
            cmd.CommandText = _sql;
            cmd.CommandTimeout = 0;
        }
    }
}