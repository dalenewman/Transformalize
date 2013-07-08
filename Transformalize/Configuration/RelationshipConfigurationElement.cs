using System.Configuration;

namespace Transformalize.Configuration {
    public class RelationshipConfigurationElement : ConfigurationElement {

        [ConfigurationProperty("leftEntity", IsRequired = true)]
        public string LeftEntity {
            get {
                return this["leftEntity"] as string;
            }
            set { this["leftEntity"] = value; }
        }

        [ConfigurationProperty("rightEntity", IsRequired = true)]
        public string RightEntity {
            get {
                return this["rightEntity"] as string;
            }
            set { this["rightEntity"] = value; }
        }

        [ConfigurationProperty("join")]
        public JoinElementCollection Join {
            get {
                return this["join"] as JoinElementCollection;
            }
        }

    }
}