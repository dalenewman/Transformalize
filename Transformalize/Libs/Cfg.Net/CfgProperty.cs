using System;

namespace Transformalize.Libs.Cfg.Net {

    public sealed class CfgProperty {

        public string Name { get; set; }
        public object Value { get; set; }
        public bool Required { get; set; }
        public bool Unique { get; set; }
        public bool Decode { get; set; }
        public bool Set { get; set; }
        public Type Type { get; set; }

        public CfgProperty(string name, object value, Type type, bool required, bool unique, bool decode) {
            Name = name;
            Value = value;
            Required = required;
            Unique = unique;
            Decode = decode;
            this.Type = type;
        }

    }
}