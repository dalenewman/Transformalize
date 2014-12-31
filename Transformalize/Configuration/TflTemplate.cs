using System.Collections.Generic;

namespace Transformalize.Configuration {

    public class TflTemplate : CfgNode {

        public TflTemplate() {
            Property(n: "name", v: string.Empty, r: true, u: true);
            Property(n: "content-type", v: "raw");
            Property(n: "file", v: string.Empty, r: true, u: true);
            Property(n: "cache", v: false);
            Property(n: "enabled", v: true);
            Property(n: "engine", v: "razor");

            Class<TflParameter>("parameters");
            Class<TflAction>("actions");
        }

        public string Name { get; set; }
        public string ContentType { get; set; }
        public string File { get; set; }
        public bool Cache { get; set; }
        public bool Enabled { get; set; }
        public string Engine { get; set; }

        public List<TflParameter> Parameters { get; set; }
        public List<TflAction> Actions { get; set; }
    }
}