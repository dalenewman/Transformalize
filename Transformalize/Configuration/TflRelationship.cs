using System.Collections.Generic;
using Transformalize.Libs.Cfg.Net;

namespace Transformalize.Configuration {
    public class TflRelationship : CfgNode {

        public TflRelationship() {

            Property(name: "left-entity", value: string.Empty, required: true, unique: false);
            Property(name: "right-entity", value: string.Empty, required: true, unique: false);
            Property(name: "left-field", value: string.Empty);
            Property(name: "right-field", value: string.Empty);
            Property(name: "index", value: false);

            Collection<TflJoin>("join");
        }

        public string LeftEntity { get; set; }
        public string RightEntity { get; set; }
        public string LeftField { get; set; }
        public string RightField { get; set; }
        public bool Index { get; set; }

        public List<TflJoin> Join { get; set; }
    }
}