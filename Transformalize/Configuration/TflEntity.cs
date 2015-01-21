using System.Collections.Generic;
using Transformalize.Libs.Cfg.Net;

namespace Transformalize.Configuration {
    public class TflEntity : CfgNode {

        [Cfg(value = "", required = false, unique = true)]
        public string Alias { get; set; }
        [Cfg(value = "input")]
        public string Connection { get; set; }
        [Cfg(value = false)]
        public bool Delete { get; set; }
        [Cfg(value = true)]
        public bool DetectChanges { get; set; }
        [Cfg(value = false)]
        public bool Group { get; set; }
        [Cfg(value = "", required = true)]
        public string Name { get; set; }
        [Cfg(value = false)]
        public bool NoLock { get; set; }
        [Cfg(value = "Default")]
        public string PipelineThreading { get; set; }
        [Cfg(value = "")]
        public string Prefix { get; set; }
        [Cfg(value = true)]
        public bool PrependProcessNameToOutputName { get; set; }
        [Cfg(value = "")]
        public string Query { get; set; }
        [Cfg(value = "")]
        public string QueryKeys { get; set; }
        [Cfg(value = 100)]
        public int Sample { get; set; }
        [Cfg(value = "")]
        public string Schema { get; set; }
        [Cfg(value = "")]
        public string Script { get; set; }
        [Cfg(value = "")]
        public string ScriptKeys { get; set; }
        [Cfg(value = false)]
        public bool TrimAll { get; set; }
        [Cfg(value = "")]
        public string Unicode { get; set; }
        [Cfg(value = "")]
        public string VariableLength { get; set; }
        [Cfg(value = "")]
        public string Version { get; set; }

        [Cfg(required = false)]
        public List<TflFilter> Filters { get; set; }
        [Cfg(required = false)]
        public List<TflField> Fields { get; set; }
        [Cfg(required = false)]
        public List<TflCalculatedField> CalculatedFields { get; set; }
        [Cfg(required = false)]
        public List<TflIo> Input { get; set; }
        [Cfg(required = false)]
        public List<TflIo> Output { get; set; }

    }
}