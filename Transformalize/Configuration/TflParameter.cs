using System;
using Transformalize.Libs.Cfg.Net;
using Transformalize.Main;

namespace Transformalize.Configuration {

    public class TflParameter : CfgNode {
        private string _type;

        [Cfg(value = "")]
        public string Entity { get; set; }
        [Cfg(value = "")]
        public string Field { get; set; }
        [Cfg(value = "")]
        public string Name { get; set; }
        [Cfg(value = null)]
        public string Value { get; set; }
        [Cfg(value = true)]
        public bool Input { get; set; }

        [Cfg(value = "string", domain = Common.ValidTypes, ignoreCase = true)]
        public string Type {
            get { return _type; }
            set { _type = value != null && value.StartsWith("sy", StringComparison.OrdinalIgnoreCase) ? value.ToLower().Replace("system.", string.Empty) : value; }
        }

        public bool HasValue() {
            return Value != null;
        }
    }

}