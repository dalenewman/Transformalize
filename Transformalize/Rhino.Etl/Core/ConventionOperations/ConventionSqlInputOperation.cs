using System.Data;
using Transformalize.Rhino.Etl.Core.Operations;

namespace Transformalize.Rhino.Etl.Core.ConventionOperations {

    public class ConventionSqlInputOperation : AbstractSqlInputOperation {
        public string Sql { get; set; }
        public int Timeout { get; set; }

        public ConventionSqlInputOperation(string connectionString)
            : base(connectionString) {
            Timeout = 0;
        }

        protected override Row CreateRowFromReader(IDataReader reader) {
            return Row.FromReader(reader);
        }

        protected override void PrepareCommand(IDbCommand cmd) {
            cmd.CommandText = Sql;
            cmd.CommandTimeout = Timeout;
        }
    }
}