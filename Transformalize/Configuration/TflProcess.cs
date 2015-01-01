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

        [Cfg(v = "", r = true, u = true)]
        public string Name { get; set; }
        [Cfg(v = true)]
        public bool Enabled { get; set; }
        [Cfg(v="")]
        public string Mode { get; set; }
        [Cfg(v=true)]
        public bool Parallel { get; set; }
        public string PipelineThreading { get; set; }
        public string Star { get; set; }
        public bool StarEnabled { get; set; }
        public string TemplateContentType { get; set; }
        public string TimeZone { get; set; }
        public string View { get; set; }
        public bool ViewEnabled { get; set; }

        public List<TflAction> Actions { get; set; }
        public List<TflCalculatedField> CalculatedFields { get; set; }
        public List<TflConnection> Connections { get; set; }
        public List<TflEntity> Entities { get; set; }
        public List<TflFileInspection> FileInspections { get; set; }
        public List<TflLog> Logs { get; set; }
        public List<TflMap> Maps { get; set; }
        public List<TflParameter> Parameters { get; set; }
        public List<TflProvider> Providers { get; set; }
        public List<TflRelationship> Relationships { get; set; }
        public List<TflSearchType> SearchTypes { get; set; }
        public List<TflTemplate> Templates { get; set; }

    }
}