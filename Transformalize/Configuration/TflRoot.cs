using System.Collections.Generic;

namespace Transformalize.Configuration {
    public class TflRoot : CfgNode {
        public TflRoot() {
            Class<TflEnvironment, string>("environments", false, "default", string.Empty);
            Class<TflProcess>("processes", true);
        }

        public List<TflEnvironment> Environments { get; set; }
        public List<TflProcess> Processes { get; set; }
    }
}