using System.Collections.Generic;
using System.Data.SqlClient;
using Transformalize.Model;
using Transformalize.Rhino.Etl.Core;
using Transformalize.Rhino.Etl.Core.Operations;

namespace Transformalize.Operations
{
    public class ResultsExperimentalLoad : AbstractOperation {
        private readonly Process _process;

        public ResultsExperimentalLoad(Process process) {
            _process = process;
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            using (var cn = new SqlConnection(_process.OutputConnection.ConnectionString)) {
                cn.Open();
                var records = 0;
                foreach (var group in rows.Partition(_process.OutputConnection.OutputBatchSize)) {
                    var sql = SqlTemplates.BatchUpdateSql(
                        group
                        , _process.OutputConnection.Year,
                        _process.Results,
                        new FieldSqlWriter().AddSurrogateKey(false).Context(),
                        _process.Output
                        );
                    var cmd = new SqlCommand(sql, cn) { CommandTimeout = 0 };
                    records += cmd.ExecuteNonQuery();
                    Info("{0} | Processed {1} rows in ResultsDaleLoad", _process.Name, records);
                }
                yield break;
            }
        }
    }
}