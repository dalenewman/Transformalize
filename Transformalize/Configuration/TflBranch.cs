using System.Collections.Generic;
using Transformalize.Main;

namespace Transformalize.Configuration {

    public class TflBranch : CfgNode {

        public TflBranch() {

            Property(n: "name", v: string.Empty, r: true, u: true);
            Property(n: "run-field", v: Common.DefaultValue);
            Property(n: "run-operator", v: "Equal");
            Property(n: "run-type", v: Common.DefaultValue);
            Property(n: "run-value", v: string.Empty);

            Class<TflTransform>("transforms");
        }

        public string Name { get; set; }
        public string RunField { get; set; }
        public string RunOperator { get; set; }
        public string RunType { get; set; }
        public string RunValue { get; set; }

        public List<TflTransform> Transforms { get; set; }
    }
}