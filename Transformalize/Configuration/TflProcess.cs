using System.Collections.Generic;
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

        [Cfg(value = "", required = true, unique = true)]
        public string Name { get; set; }
        [Cfg(value = true)]
        public bool Enabled { get; set; }
        [Cfg(value="")]
        public string Mode { get; set; }
        [Cfg(value=true)]
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