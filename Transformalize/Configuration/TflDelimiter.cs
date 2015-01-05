using Transformalize.Libs.Cfg.Net;

namespace Transformalize.Configuration {

    public class TflDelimiter : CfgNode {
        public TflDelimiter() {
            Property(name:"character", value:default(char), required:true, unique:true, decode:true);
            Property(name:"name", value:string.Empty, required:true);
        }
    }
}