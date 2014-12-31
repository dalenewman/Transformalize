using System.Collections.Generic;

namespace Transformalize.Configuration {
    public class TflRelationship : CfgNode {

        public TflRelationship() {

            Property(n: "left-entity", v: string.Empty, r: true, u: false);
            Property(n: "right-entity", v: string.Empty, r: true, u: false);
            Property(n: "left-field", v: string.Empty);
            Property(n: "right-field", v: string.Empty);
            Property(n: "index", v: false);

            Class<TflJoin>("join");
        }

        public string LeftEntity { get; set; }
        public string RightEntity { get; set; }
        public string LeftField { get; set; }
        public string RightField { get; set; }
        public bool Index { get; set; }

        public List<TflJoin> Join { get; set; }
    }
}