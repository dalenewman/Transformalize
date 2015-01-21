using System.Collections.Generic;
using Transformalize.Libs.Cfg.Net;

namespace Transformalize.Configuration {
    public class TflProcess : CfgNode {

        [Cfg( value = "", required = true, unique = true)]
        public string Name { get; set; }
        [Cfg( value = true)]
        public bool Enabled { get; set; }
        [Cfg( value = "")]
        public string Mode { get; set; }
        [Cfg( value = true)]
        public bool Parallel { get; set; }
        [Cfg( value = "")]
        public string PipelineThreading { get; set; }
        [Cfg( value = "")]
        public string Star { get; set; }
        [Cfg( value = true)]
        public bool StarEnabled { get; set; }
        [Cfg( value = "raw", domain = "raw,html")]
        public string TemplateContentType { get; set; }
        [Cfg( value = "")]
        public string TimeZone { get; set; }
        [Cfg( value = "")]
        public string View { get; set; }
        [Cfg( value = true)]
        public bool ViewEnabled { get; set; }

        [Cfg()]
        public List<TflAction> Actions { get; set; }
        [Cfg()]
        public List<TflCalculatedField> CalculatedFields { get; set; }
        [Cfg(required = true)]
        public List<TflConnection> Connections { get; set; }
        [Cfg(required = true)]
        public List<TflEntity> Entities { get; set; }
        [Cfg()]
        public List<TflFileInspection> FileInspection { get; set; }
        [Cfg(sharedProperty = "rows", sharedValue = 10000)]
        public List<TflLog> Log { get; set; }
        [Cfg()]
        public List<TflMap> Maps { get; set; }
        [Cfg()]
        public List<TflProvider> Providers { get; set; }
        [Cfg()]
        public List<TflRelationship> Relationships { get; set; }
        [Cfg()]
        public List<TflScript> Scripts { get; set; }
        [Cfg()]
        public List<TflSearchType> SearchTypes { get; set; }
        [Cfg()]
        public List<TflTemplate> Templates { get; set; }

    }
}