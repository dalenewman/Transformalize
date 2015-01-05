using System.Collections.Generic;
using Transformalize.Libs.Cfg.Net;

namespace Transformalize.Configuration {
    public class TflEntity : CfgNode {

        public TflEntity() {

            Property(name: "alias", value: string.Empty, required: false, unique: true);
            Property(name: "connection", value: "input");
            Property(name: "delete", value: false);
            Property(name: "detect-changes", value: true);
            Property(name: "group", value: false);
            Property(name: "name", value: string.Empty, required: true);
            Property(name: "no-lock", value: false);
            Property(name: "pipeline-threading", value: "Default");
            Property(name: "prefix", value: string.Empty);
            Property(name: "prepend-process-name-to-output-name", value: true);
            Property(name: "sample", value: 100);
            Property(name: "schema", value: string.Empty);
            Property(name: "query-keys", value: string.Empty);
            Property(name: "query", value: string.Empty);
            Property(name: "script-keys", value: string.Empty);
            Property(name: "script", value: string.Empty);
            Property(name: "trim-all", value: false);
            Property(name: "unicode", value: string.Empty);
            Property(name: "variable-length", value: string.Empty);
            Property(name: "version", value: string.Empty);

            Collection<TflFilter>("filter");
            Collection<TflField>("fields");
            Collection<TflCalculatedField>("calculated-fields");
            Collection<TflIo>("input");
            Collection<TflIo>("output");
        }

        public string Alias { get; set; }
        public string Connection { get; set; }
        public bool Delete { get; set; }
        public bool DetectChanges { get; set; }
        public bool Group { get; set; }
        public string Name { get; set; }
        public bool NoLock { get; set; }
        public string PipelineThreading { get; set; }
        public string Prefix { get; set; }
        public bool PrependProcessNameToOutputName { get; set; }
        public int Sample { get; set; }
        public string Schema { get; set; }
        public string QueryKeys { get; set; }
        public string Query { get; set; }
        public string ScriptKeys { get; set; }
        public string Script { get; set; }
        public bool TrimAll { get; set; }
        public string Unicode { get; set; }
        public string VariableLength { get; set; }
        public string Version { get; set; }

        public List<TflFilter> Filters { get; set; }
        public List<TflField> Fields { get; set; }
        public List<TflCalculatedField> CalculatedFields { get; set; }
        public List<TflIo> Input { get; set; }
        public List<TflIo> Output { get; set; } 

    }
}