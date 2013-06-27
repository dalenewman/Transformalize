using System.Configuration;

namespace Transformalize.Configuration
{
    public class TransformConfigurationElement : ConfigurationElement {

        [ConfigurationProperty("method", IsRequired = true)]
        public string Method {
            get {
                return this["method"] as string;
            }
            set { this["method"] = value; }
        }

        [ConfigurationProperty("value", IsRequired = false)]
        public string Value {
            get {
                return this["value"] as string;
            }
            set { this["value"] = value; }
        }

        [ConfigurationProperty("oldValue", IsRequired = false)]
        public string OldValue {
            get {
                return this["oldValue"] as string;
            }
            set { this["oldValue"] = value; }
        }


        [ConfigurationProperty("newValue", IsRequired = false)]
        public string NewValue {
            get {
                return this["newValue"] as string;
            }
            set { this["newValue"] = value; }
        }

        [ConfigurationProperty("trimChars", IsRequired = false)]
        public string TrimChars {
            get {
                return this["trimChars"] as string;
            }
            set { this["trimChars"] = value; }
        }

        [ConfigurationProperty("index", IsRequired = false)]
        public int Index {
            get {
                return (int) this["index"];
            }
            set { this["index"] = value; }
        }

        [ConfigurationProperty("startIndex", IsRequired = false)]
        public int StartIndex {
            get {
                return (int)this["startIndex"];
            }
            set { this["startIndex"] = value; }
        }

        [ConfigurationProperty("length", IsRequired = false)]
        public int Length {
            get {
                return (int)this["length"];
            }
            set { this["length"] = value; }
        }

        [ConfigurationProperty("map", IsRequired = false)]
        public string Map {
            get {
                return this["map"] as string;
            }
            set { this["map"] = value; }
        }


        [ConfigurationProperty("script", IsRequired = false)]
        public string Script {
            get {
                return this["script"] as string;
            }
            set { this["script"] = value; }
        }
    }
}