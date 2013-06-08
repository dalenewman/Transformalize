namespace Transformalize.Model {
    public class Connection {
        public string ConnectionString { get; set; }
        public string Provider { get; set; }
        public int Year { get; set; }
        public int BatchInsertSize { get; set; }
        public int BatchUpdateSize { get; set; }
        public int BulkInsertSize { get; set; }
        public int BatchSelectSize { get; set; }
    }
}