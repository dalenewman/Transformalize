using System.Collections.Generic;
using System.Data;
using Transformalize.Libs.Dapper;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Main.Providers;
using Transformalize.Operations.Transform;

namespace Transformalize.Main {
    public class RunOperation : ShouldRunOperation {
        private readonly int _timeOut;
        private readonly IDbConnection _connection;

        public RunOperation(string inKey, AbstractConnection connection, int timeOut)
            : base(inKey, string.Empty)
        {
            _timeOut = timeOut;
            _connection = connection.GetConnection();
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            OnFinishedProcessing += RunOperation_OnFinishedProcessing;
            _connection.Open();
            foreach (var row in rows) {
                if (ShouldRun(row)) {
                    var command = row[InKey].ToString();
                    _connection.Execute(command, commandTimeout:_timeOut);
                    TflLogger.Info(ProcessName, EntityName, "Executed {0}", command);
                }
                yield return row;
            }
        }

        void RunOperation_OnFinishedProcessing(IOperation obj) {
            if (_connection.State == ConnectionState.Open) {
                _connection.Close();
            }
        }
    }
}