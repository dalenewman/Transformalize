using Transformalize.Libs.Cfg.Net;

namespace Transformalize.Configuration {

    public class TflMap : CfgNode {

        public TflMap() {

            Property(name: "name", value: string.Empty, required:true, unique:true);
            Property(name: "connection", value: "input");
            Property(name: "query", value: string.Empty);

            Collection<TflMapItem>("items");
        }

        public string Name { get; set; }
        public string Connection { get; set; }
        public string Query { get; set; }
    }
}