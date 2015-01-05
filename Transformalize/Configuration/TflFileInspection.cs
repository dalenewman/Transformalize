using System.Collections.Generic;
using Transformalize.Libs.Cfg.Net;

namespace Transformalize.Configuration {
    public class TflFileInspection : CfgNode {

        public TflFileInspection() {

            Property(name: "name", value: string.Empty, required: true, unique: true);
            Property(name: "sample", value: 100);
            Property(name: "max-length", value: 0);
            Property(name: "min-length", value: 0);

            Collection<TflType>("types");
            Collection<TflDelimiter>("delimiters", true);
        }

        public string Name { get; set; }
        public int Sample { get; set; }
        public int MaxLength { get; set; }
        public int MinLength { get; set; }

        public List<TflType> Types { get; set; }
        public List<TflDelimiter> Delimiters { get; set; }
    }
}