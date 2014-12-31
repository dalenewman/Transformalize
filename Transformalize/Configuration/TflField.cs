using System.Collections.Generic;
using Transformalize.Main;

namespace Transformalize.Configuration {
    public class TflField : CfgNode {

        public TflField() {

            Property(n: "aggregate", v: string.Empty);
            Property(n: "alias", v: string.Empty, r: false, u: true);
            Property(n: "default", v: string.Empty);
            Property(n: "default-blank", v: false);
            Property(n: "default-empty", v: false);
            Property(n: "default-white-space", v: false);
            Property(n: "delimiter", v: ", ");
            Property(n: "distinct", v: false);
            Property(n: "index", v: short.MaxValue);
            Property(n: "input", v: true);
            Property(n: "label", v: string.Empty);
            Property(n: "length", v: "64");
            Property(n: "name", v: string.Empty, r: true);
            Property(n: "node-type", v: "element");
            Property(n: "optional", v: false);
            Property(n: "output", v: true);
            Property(n: "precision", v: 18);
            Property(n: "primary-key", v: false);
            Property(n: "quoted-with", v: default(char));
            Property(n: "raw", v: false);
            Property(n: "read-inner-xml", v: true);
            Property(n: "scale", v: 9);
            Property(n: "search-type", v: "default");
            Property(n: "sort", v: string.Empty);
            Property(n: "t", v: string.Empty);
            Property(n: "type", v: "string");
            Property(n: "unicode", v: Common.DefaultValue);
            Property(n: "variable-length", v: Common.DefaultValue);

            Class<TflNameReference>("search-types");
            Class<TflTransform>("transforms");
        }

        public bool DefaultBlank { get; set; }
        public bool DefaultEmpty { get; set; }
        public bool DefaultWhiteSpace { get; set; }
        public bool Distinct { get; set; }
        public bool Input { get; set; }
        public bool Optional { get; set; }
        public bool Output { get; set; }
        public bool PrimaryKey { get; set; }
        public bool Raw { get; set; }
        public bool ReadInnerXml { get; set; }
        public bool Unicode { get; set; }
        public bool VariableLength { get; set; }
        public char QuotedWith { get; set; }
        public int Index { get; set; }
        public int Precision { get; set; }
        public int Scale { get; set; }
        public string Aggregate { get; set; }
        public string Alias { get; set; }
        public string Default { get; set; }
        public string Delimiter { get; set; }
        public string Label { get; set; }
        public string Length { get; set; }
        public string Name { get; set; }
        public string NodeType { get; set; }
        public string SearchType { get; set; }
        public string Sort { get; set; }
        public string T { get; set; }
        public string Type { get; set; }

        public List<TflNameReference> SearchTypes { get; set; }
        public List<TflTransform> Transforms { get; set; } 

    }
}