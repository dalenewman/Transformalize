namespace Transformalize {
    public class OperationSignature {

        public OperationSignature() {
            Parameters = new List<OperationParameter>();
            Parameters = new List<OperationParameter>();
        }

        public OperationSignature(string method) {
            Method = method;
            Parameters = new List<OperationParameter>();
        }

        public string Method { get; set; }

        public List<OperationParameter> Parameters { get; set; }
    }
}