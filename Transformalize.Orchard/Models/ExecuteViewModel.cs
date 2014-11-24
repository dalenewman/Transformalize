namespace Transformalize.Orchard.Models {
    public class ExecuteViewModel {
        public TransformalizeResponse TransformalizeResponse { get; set; }
        public bool DisplayLog { get; set; }
        public ExecuteViewModel() {
            TransformalizeResponse = new TransformalizeResponse();
        }
    }
}