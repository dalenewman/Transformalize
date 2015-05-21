using System.Collections.Generic;
using Transformalize.Libs.Cfg.Net;

namespace Transformalize.Configuration {

    public class TflMap : CfgNode {

        [Cfg(value = "input", toLower = true)]
        public string Connection { get; set; }
        [Cfg(required = true, unique = true)]
        public string Name { get; set; }
        [Cfg(value = "")]
        public string Query { get; set; }

        [Cfg(required = false)]
        public List<TflMapItem> Items { get; set; }

        protected override void Validate() {
            if (Items.Count == 0 && Query == string.Empty) {
                Error(string.Format("Map '{0}' needs items or a query.", Name));
            }
        }
    }
}