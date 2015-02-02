using System.Collections.Generic;
using Transformalize.Libs.Cfg.Net;
using Transformalize.Main;

namespace Transformalize.Configuration {

    public class TflBranch : CfgNode {

        [Cfg(unique = true)]
        public string Name { get; set; }
        [Cfg(value = Common.DefaultValue)]
        public string RunField { get; set; }
        [Cfg(value = "Equal", domain = Common.ValidComparisons)]
        public string RunOperator { get; set; }
        [Cfg(value = Common.DefaultValue)]
        public string RunType { get; set; }
        [Cfg(value = "")]
        public string RunValue { get; set; }

        [Cfg()]
        public List<TflTransform> Transforms { get; set; }

    }
}