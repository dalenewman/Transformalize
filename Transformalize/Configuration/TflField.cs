using System.Collections.Generic;
using Transformalize.Libs.Cfg.Net;
using Transformalize.Main;

namespace Transformalize.Configuration {
    public class TflField : CfgNode {

        public TflField() {

            Property(name: "aggregate", value: string.Empty);
            Property(name: "alias", value: string.Empty, required: false, unique: true);
            Property(name: "default", value: string.Empty);
            Property(name: "default-blank", value: false);
            Property(name: "default-empty", value: false);
            Property(name: "default-white-space", value: false);
            Property(name: "delimiter", value: ", ");
            Property(name: "distinct", value: false);
            Property(name: "index", value: short.MaxValue);
            Property(name: "input", value: true);
            Property(name: "label", value: string.Empty);
            Property(name: "length", value: "64");
            Property(name: "name", value: string.Empty, required: true);
            Property(name: "node-type", value: "element");
            Property(name: "optional", value: false);
            Property(name: "output", value: true);
            Property(name: "precision", value: 18);
            Property(name: "primary-key", value: false);
            Property(name: "quoted-with", value: default(char));
            Property(name: "raw", value: false);
            Property(name: "read-inner-xml", value: true);
            Property(name: "scale", value: 9);
            Property(name: "search-type", value: "default");
            Property(name: "sort", value: string.Empty);
            Property(name: "t", value: string.Empty);
            Property(name: "type", value: "string");
            Property(name: "unicode", value: Common.DefaultValue);
            Property(name: "variable-length", value: Common.DefaultValue);

            Collection<TflNameReference>("search-types");
            Collection<TflTransform>("transforms");
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
        public string Unicode { get; set; }
        public string VariableLength { get; set; }
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