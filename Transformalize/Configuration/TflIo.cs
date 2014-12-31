using Transformalize.Main;

namespace Transformalize.Configuration {
    public class TflIo : CfgNode {
        public TflIo() {
            Property(n: "name", v: string.Empty);
            Property(n: "connection", v: string.Empty, r: true);
            Property(n: "run-field", v: string.Empty);
            Property(n: "run-type", v: Common.DefaultValue);
            Property(n: "run-operator", v: "Equal");
            Property(n: "run-value", v: string.Empty);
        }

        public string Name { get; set; }
        public string Connection { get; set; }
        public string RunField { get; set; }
        public string RunType { get; set; }
        public string RunOperator { get; set; }
        public string RunValue { get; set; }
    }
}