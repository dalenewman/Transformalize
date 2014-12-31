using System.Collections.Generic;

namespace Transformalize.Configuration {
    public class TflProcess : CfgNode {

        public TflProcess() {

            Property(n: "name", v: string.Empty, r: true, u: true);
            Property(n: "enabled", v: true);
            Property(n: "mode", v: string.Empty);
            Property(n: "parallel", v: true);
            Property(n: "pipeline-threading", v: string.Empty);
            Property(n: "star", v: string.Empty);
            Property(n: "star-enabled", v: true);
            Property(n: "template-content-type", v: "raw");
            Property(n: "time-zone", v: string.Empty);
            Property(n: "view", v: string.Empty);
            Property(n: "view-enabled", v: true);

            Class<TflAction>("actions");
            Class<TflCalculatedField>("calculated-fields");
            Class<TflConnection>("connections", true);
            Class<TflEntity>("entities", true);
            Class<TflFileInspection>("file-inspection");
            Class<TflLog, int>("log", false, "rows", 10000);
            Class<TflMap>("maps");
            Class<TflParameter>("parameters");
            Class<TflProvider>("providers");
            Class<TflRelationship>("relationships");
            Class<TflScript>("scripts");
            Class<TflSearchType>("search-types");
            Class<TflTemplate>("templates");
        }

        public List<TflAction> Actions { get; set; }

        public string Name { get; set; }
        public bool Enabled { get; set; }
        public string Mode { get; set; }
        public bool Parallel { get; set; }
        public string PipelineThreading { get; set; }
        public string Star { get; set; }
        public bool StarEnabled { get; set; }
        public string TemplateContentType { get; set; }
        public string TimeZone { get; set; }
        public string View { get; set; }
        public bool ViewEnabled { get; set; }

    }
}