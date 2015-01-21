using System.Collections.Generic;
using Transformalize.Libs.Cfg.Net;
using Transformalize.Main;

namespace Transformalize.Configuration {
    public class TflField : CfgNode {

        [Cfg(value=false)]
        public bool DefaultBlank { get; set; }
        [Cfg(value = false)]
        public bool DefaultEmpty { get; set; }
        [Cfg(value = false)]
        public bool DefaultWhiteSpace { get; set; }
        [Cfg(value = false)]
        public bool Distinct { get; set; }
        [Cfg(value = true)]
        public bool Input { get; set; }
        [Cfg(value = false)]
        public bool Optional { get; set; }
        [Cfg(value = true)]
        public bool Output { get; set; }
        [Cfg(value = false)]
        public bool PrimaryKey { get; set; }
        [Cfg(value = false)]
        public bool Raw { get; set; }
        [Cfg(value = true)]
        public bool ReadInnerXml { get; set; }
        [Cfg(value = default(char))]
        public char QuotedWith { get; set; }
        [Cfg(value=short.MaxValue)]
        public int Index { get; set; }
        [Cfg(value=18)]
        public int Precision { get; set; }
        [Cfg(value=9)]
        public int Scale { get; set; }
        [Cfg(value="")]
        public string Aggregate { get; set; }
        [Cfg(value = "", required = false, unique = true)]
        public string Alias { get; set; }
        [Cfg(value = "")]
        public string Default { get; set; }
        [Cfg(value = ", ")]
        public string Delimiter { get; set; }
        [Cfg(value = "")]
        public string Label { get; set; }
        [Cfg(value = "64")]
        public string Length { get; set; }
        [Cfg(required = true)]
        public string Name { get; set; }
        [Cfg(value="element")]
        public string NodeType { get; set; }
        [Cfg(value = "default")]
        public string SearchType { get; set; }
        [Cfg(value="")]
        public string Sort { get; set; }
        [Cfg(value="")]
        public string T { get; set; }
        [Cfg(value="string")]
        public string Type { get; set; }
        [Cfg(value=Common.DefaultValue)]
        public string Unicode { get; set; }
        [Cfg(value=Common.DefaultValue)]
        public string VariableLength { get; set; }

        [Cfg(required = false)]
        public List<TflNameReference> SearchTypes { get; set; }
        [Cfg(required = false)]
        public List<TflTransform> Transforms { get; set; } 

    }
}