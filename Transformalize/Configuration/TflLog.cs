using Transformalize.Main;

namespace Transformalize.Configuration {
    public class TflLog : CfgNode {
        public TflLog() {

            Property(n: "name", v: string.Empty, r:true, u:true);
            Property(n: "provider", v: Common.DefaultValue);
            Property(n: "layout", v: Common.DefaultValue);
            Property(n: "level", v: "Informational");
            Property(n: "connection", v: Common.DefaultValue);
            Property(n: "from", v: Common.DefaultValue);
            Property(n: "to", v: Common.DefaultValue);
            Property(n: "subject", v: Common.DefaultValue);
            Property(n: "file", v: Common.DefaultValue);
            Property(n: "folder", v: Common.DefaultValue);
            Property(n: "async", v: false);
        }

        public string Name { get; set; }
        public string Provider { get; set; }
        public string Layout { get; set; }
        public string Level { get; set; }
        public string Connection { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Subject { get; set; }
        public string File { get; set; }
        public string Folder { get; set; }
        public bool Async { get; set; }
    }
}