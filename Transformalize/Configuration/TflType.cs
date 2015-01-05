using Transformalize.Libs.Cfg.Net;

namespace Transformalize.Configuration {
    public class TflType : CfgNode {
        public TflType() {
            Property(name:"type", value:string.Empty, required:true, unique:true);
        }
    }
}