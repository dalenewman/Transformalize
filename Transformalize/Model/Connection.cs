using Transformalize.Data;

namespace Transformalize.Model {
    public class Connection {
        private readonly IConnectionChecker _connectionChecker;

        public Connection(IConnectionChecker connectionChecker) {
            _connectionChecker = connectionChecker;
        }

        public string ConnectionString { get; set; }
        public string Provider { get; set; }
        public int Year { get; set; }
        public int OutputBatchSize { get; set; }
        public int InputBatchSize { get; set; }
        public bool IsReady() {
            return _connectionChecker.Check(ConnectionString);
        }
    }
}