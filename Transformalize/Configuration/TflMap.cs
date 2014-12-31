namespace Transformalize.Configuration {

    public class TflMap : CfgNode {

        public TflMap() {

            Property(n: "name", v: string.Empty, r:true, u:true);
            Property(n: "connection", v: "input");
            Property(n: "query", v: string.Empty);

            Class<TflMapItem>("items");
        }

        public string Name { get; set; }
        public string Connection { get; set; }
        public string Query { get; set; }
    }
}