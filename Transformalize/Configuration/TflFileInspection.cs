using System.Collections.Generic;

namespace Transformalize.Configuration {
    public class TflFileInspection : CfgNode {

        public TflFileInspection() {

            Property(n: "name", v: string.Empty, r: true, u: true);
            Property(n: "sample", v: 100);
            Property(n: "max-length", v: 0);
            Property(n: "min-length", v: 0);

            Class<TflType>("types");
            Class<TflDelimiter>("delimiters", true);
        }

        public string Name { get; set; }
        public int Sample { get; set; }
        public int MaxLength { get; set; }
        public int MinLength { get; set; }

        public List<TflType> Types { get; set; }
        public List<TflDelimiter> Delimiters { get; set; }
    }
}