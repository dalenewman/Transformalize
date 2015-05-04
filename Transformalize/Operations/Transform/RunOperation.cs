using System.Collections.Generic;
using System.Data;
using Transformalize.Libs.Dapper;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Logging;
using Transformalize.Main.Providers;

namespace Transformalize.Operations.Transform {

    public class RunOperation : ShouldRunOperation {
        private readonly int _timeOut;
        private readonly IDbConnection _connection;
        private readonly ILogger _logger;

        public RunOperation(string inKey, AbstractConnection connection, int timeOut)
            : base(inKey, string.Empty) {
            _timeOut = timeOut;
            _connection = connection.GetConnection();
            _logger = connection.Logger;
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            OnFinishedProcessing += RunOperation_OnFinishedProcessing;
            _connection.Open();
            foreach (var row in rows) {
                if (ShouldRun(row)) {
                    var command = row[InKey].ToString();
                    _connection.Execute(command, commandTimeout: _timeOut);

                    _logger.EntityInfo(EntityName, "Executed {0}", command);
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