using Transformalize.Libs.Cfg.Net;

namespace Transformalize.Configuration {
    public class TflProcess : CfgNode {

        public TflProcess() {

            Property(name: "name", value: string.Empty, required: true, unique: true);
            Property(name: "enabled", value: true);
            Property(name: "mode", value: string.Empty);
            Property(name: "parallel", value: true);
            Property(name: "pipeline-threading", value: string.Empty);
            Property(name: "star", value: string.Empty);
            Property(name: "star-enabled", value: true);
            Property(name: "template-content-type", value: "raw");
            Property(name: "time-zone", value: string.Empty);
            Property(name: "view", value: string.Empty);
            Property(name: "view-enabled", value: true);

            Collection<TflAction>("actions");
            Collection<TflCalculatedField>("calculated-fields");
            Collection<TflConnection>("connections", true);
            Collection<TflEntity>("entities", true);
            Collection<TflFileInspection>("file-inspection");
            Collection<TflLog, int>("log", false, "rows", 10000);
            Collection<TflMap>("maps");
            Collection<TflParameter>("parameters");
            Collection<TflProvider>("providers");
            Collection<TflRelationship>("relationships");
            Collection<TflScript>("scripts");
            Collection<TflSearchType>("search-types");
            Collection<TflTemplate>("templates");
        }

    }
}