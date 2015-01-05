using System.Collections.Generic;
using Transformalize.Libs.Cfg.Net;

namespace Transformalize.Configuration {
    public class TflRoot : CfgNode {
        public TflRoot() {
            TurnOffProperties = true;
            Collection<TflEnvironment, string>("environments", false, "default", string.Empty);
            Collection<TflProcess>("processes", true);
        }

    }
}