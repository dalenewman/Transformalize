using System.Collections.Generic;
using Transformalize.Libs.Cfg.Net;
using Transformalize.Main;

namespace Transformalize.Configuration {

    public class TflBranch : CfgNode {

        public TflBranch() {

            Property(name: "name", value: string.Empty, required: true, unique: true);
            Property(name: "run-field", value: Common.DefaultValue);
            Property(name: "run-operator", value: "Equal");
            Property(name: "run-type", value: Common.DefaultValue);
            Property(name: "run-value", value: string.Empty);

            Collection<TflTransform>("transforms");
        }
    }
}