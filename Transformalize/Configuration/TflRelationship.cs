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

    }
}