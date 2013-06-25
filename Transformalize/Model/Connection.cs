namespace Transformalize.Model {
    public class Connection {
        public string ConnectionString { get; set; }
        public string Provider { get; set; }
        public int Year { get; set; }
        public int OutputBatchSize { get; set; }
        public int InputBatchSize { get; set; }
    }
}