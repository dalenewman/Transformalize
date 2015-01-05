using System.Collections.Generic;
using Transformalize.Libs.Cfg.Net;

namespace Transformalize.Configuration {

    public class TflTemplate : CfgNode {

        public TflTemplate() {
            Property(name: "name", value: string.Empty, required: true, unique: true);
            Property(name: "content-type", value: "raw");
            Property(name: "file", value: string.Empty, required: true, unique: true);
            Property(name: "cache", value: false);
            Property(name: "enabled", value: true);
            Property(name: "engine", value: "razor");

            Collection<TflParameter>("parameters");
            Collection<TflAction>("actions");
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