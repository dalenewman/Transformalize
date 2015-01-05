using Transformalize.Libs.Cfg.Net;
using Transformalize.Main;

namespace Transformalize.Configuration {
    public class TflLog : CfgNode {
        public TflLog() {

            Property(name: "name", value: string.Empty, required:true, unique:true);
            Property(name: "provider", value: Common.DefaultValue);
            Property(name: "layout", value: Common.DefaultValue);
            Property(name: "level", value: "Informational");
            Property(name: "connection", value: Common.DefaultValue);
            Property(name: "from", value: Common.DefaultValue);
            Property(name: "to", value: Common.DefaultValue);
            Property(name: "subject", value: Common.DefaultValue);
            Property(name: "file", value: Common.DefaultValue);
            Property(name: "folder", value: Common.DefaultValue);
            Property(name: "async", value: false);
        }

    }
}