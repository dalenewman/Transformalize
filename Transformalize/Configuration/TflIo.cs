using Transformalize.Libs.Cfg.Net;
using Transformalize.Main;

namespace Transformalize.Configuration {
    public class TflIo : CfgNode {
        public TflIo() {
            Property(name: "name", value: string.Empty);
            Property(name: "connection", value: string.Empty, required: true);
            Property(name: "run-field", value: string.Empty);
            Property(name: "run-type", value: Common.DefaultValue);
            Property(name: "run-operator", value: "Equal");
            Property(name: "run-value", value: string.Empty);
        }

        public string Name { get; set; }
        public string Connection { get; set; }
        public string RunField { get; set; }
        public string RunType { get; set; }
        public string RunOperator { get; set; }
        public string RunValue { get; set; }
    }
}