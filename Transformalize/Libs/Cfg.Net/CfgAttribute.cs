using System;

namespace Transformalize.Libs.Cfg.Net {
    [AttributeUsage(AttributeTargets.Property)]
    public class CfgAttribute : Attribute {
        public object value { get; set; }
        public bool required { get; set; }
        public bool unique { get; set; }
        public bool decode { get; set; }
        public string sharedProperty { get; set; }
        public object sharedValue { get; set; }
    }
}