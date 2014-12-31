using System.Collections.Generic;

namespace Transformalize.Configuration {
    public class TflEntity : CfgNode {

        public TflEntity() {

            Property(n: "alias", v: string.Empty, r: false, u: true);
            Property(n: "connection", v: "input");
            Property(n: "delete", v: false);
            Property(n: "detect-changes", v: true);
            Property(n: "group", v: false);
            Property(n: "name", v: string.Empty, r: true);
            Property(n: "no-lock", v: false);
            Property(n: "pipeline-threading", v: "Default");
            Property(n: "prefix", v: string.Empty);
            Property(n: "prepend-process-name-to-output-name", v: true);
            Property(n: "sample", v: 100);
            Property(n: "schema", v: string.Empty);
            Property(n: "query-keys", v: string.Empty);
            Property(n: "query", v: string.Empty);
            Property(n: "script-keys", v: string.Empty);
            Property(n: "script", v: string.Empty);
            Property(n: "trim-all", v: false);
            Property(n: "unicode", v: string.Empty);
            Property(n: "variable-length", v: string.Empty);
            Property(n: "version", v: string.Empty);

            Class<TflFilter>("filter");
            Class<TflField>("fields");
            Class<TflCalculatedField>("calculated-fields");
            Class<TflIo>("input");
            Class<TflIo>("output");
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
        public bool Unicode { get; set; }
        public bool VariableLength { get; set; }
        public string Version { get; set; }

        public List<TflFilter> Filters { get; set; }
        public List<TflField> Fields { get; set; }
        public List<TflCalculatedField> CalculatedFields { get; set; }
        public List<TflIo> Input { get; set; }
        public List<TflIo> Output { get; set; } 

    }
}