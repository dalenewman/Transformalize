using System.Configuration;

namespace Transformalize.Configuration
{
    public class FieldConfigurationElement : ConfigurationElement {

        [ConfigurationProperty("name", IsRequired = true)]
        public string Name {
            get {
                return this["name"] as string;
            }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("alias", IsRequired = false, DefaultValue = "")]
        public string Alias {
            get {
                var alias = this["alias"] as string;
                return alias == null || alias.Equals(string.Empty) ? Name : alias;
            }
            set { this["alias"] = value; }
        }

        [ConfigurationProperty("type", IsRequired = false, DefaultValue = "System.String")]
        public string Type {
            get {
                return this["type"] as string;
            }
            set { this["type"] = value; }
        }

        [ConfigurationProperty("xml")]
        public XmlElementCollection Xml {
            get {
                return this["xml"] as XmlElementCollection;
            }
        }

        [ConfigurationProperty("length", IsRequired = false, DefaultValue = 0)]
        public int Length {
            get {
                return (int)this["length"];
            }
            set { this["length"] = value; }
        }

        [ConfigurationProperty("precision", IsRequired = false, DefaultValue = 18)]
        public int Precision {
            get {
                return (int)this["precision"];
            }
            set { this["precision"] = value; }
        }

        [ConfigurationProperty("scale", IsRequired = false, DefaultValue = 9)]
        public int Scale {
            get {
                return (int)this["scale"];
            }
            set { this["scale"] = value; }
        }

        [ConfigurationProperty("input", IsRequired = false, DefaultValue = true)]
        public bool Input {
            get {
                return (bool)this["input"];
            }
            set { this["input"] = value; }
        }

        [ConfigurationProperty("output", IsRequired = false, DefaultValue = true)]
        public bool Output {
            get {
                return (bool) this["output"];
            }
            set { this["output"] = value; }
        }

        [ConfigurationProperty("default", IsRequired = false, DefaultValue = "")]
        public string Default {
            get {
                return (string) this["default"];
            }
            set { this["default"] = value; }
        }

        [ConfigurationProperty("transforms")]
        public TransformElementCollection Transforms {
            get {
                return this["transforms"] as TransformElementCollection;
            }
        }
    }
}