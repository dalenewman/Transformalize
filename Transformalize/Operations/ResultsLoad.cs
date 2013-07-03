using System.Data.SqlClient;
using Transformalize.Model;
using Transformalize.Rhino.Etl.Core;
using Transformalize.Rhino.Etl.Core.Operations;

namespace Transformalize.Operations
{
    public class ResultsLoad : SqlBatchOperation {
        private readonly Process _process;

        public ResultsLoad(Process process) : base(process.OutputConnection.ConnectionString) {
            _process = process;
            BatchSize = 50;
            UseTransaction = false;
        }

        protected override void PrepareCommand(Row row, SqlCommand command) {
            var sets = new FieldSqlWriter(_process.Results).Alias().SetParam().Write();
            command.CommandText = string.Format("UPDATE {0} SET {1} WHERE TflKey = @TflKey;", _process.Output, sets);
            foreach (var r in _process.Results) {
                AddParameter(command, r.Key, row[r.Key]);
            }
            AddParameter(command, "TflKey", row["TflKey"]);
        }
    }
}