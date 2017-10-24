namespace Transformalize {
    public class OperationParameter {

        public OperationParameter() {

        }

        public OperationParameter(string name) {
            Name = name;
        }

        public OperationParameter(string name, string value) {
            Name = name;
            Value = value;
        }

        public string Name { get; set; }
        public string Value { get; set; }
    }
}