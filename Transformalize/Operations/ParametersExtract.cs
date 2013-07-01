using System.Data;
using Transformalize.Model;
using Transformalize.Rhino.Etl.Core;
using Transformalize.Rhino.Etl.Core.Operations;

namespace Transformalize.Operations
{
    public class ParametersExtract : InputCommandOperation {
        private readonly Process _process;

        public ParametersExtract(Process process)
            : base(process.OutputConnection.ConnectionString) {
            _process = process;
            UseTransaction = false;
            }

        protected override Row CreateRowFromReader(IDataReader reader) {
            return Row.FromReader(reader);
        }

        protected override void PrepareCommand(IDbCommand cmd) {
            var fields = new FieldSqlWriter(_process.Parameters).Alias().Write();
            cmd.CommandText = string.Format("SELECT [TflKey], {0} FROM {1};", fields, _process.Output);
            Debug("SQL: {0}", cmd.CommandText);
            cmd.CommandTimeout = 0;
        }
    }
}