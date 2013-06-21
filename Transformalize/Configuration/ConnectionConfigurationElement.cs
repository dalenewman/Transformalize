using System.Configuration;

namespace Transformalize.Configuration
{
    public class ConnectionConfigurationElement : ConfigurationElement {

        [ConfigurationProperty("name", IsRequired = true)]
        public string Name {
            get {
                return this["name"] as string;
            }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("value", IsRequired = true)]
        public string Value {
            get {
                return this["value"] as string;
            }
            set { this["value"] = value; }
        }

        [ConfigurationProperty("year", IsRequired = false, DefaultValue = 2008)]
        public int Year {
            get {
                return (int) this["year"];
            }
            set { this["year"] = value; }
        }

        [ConfigurationProperty("provider", IsRequired = false, DefaultValue = "")]
        public string Provider {
            get {
                return this["provider"] as string;
            }
            set { this["provider"] = value; }
        }

        /// <summary>
        /// When creating temporary tables via VALUES... or SELECT & UNION ALL, too long of a statement can slow performance, 50 is a good default for this.
        /// </summary>
        [ConfigurationProperty("batchInsertSize", IsRequired = false, DefaultValue = 50)]
        public int BatchInsertSize {
            get {
                return (int)this["batchInsertSize"];
            }
            set { this["batchInsertSize"] = value; }
        }

        [ConfigurationProperty("bulkInsertSize", IsRequired = false, DefaultValue = 100)]
        public int BulkInsertSize {
            get {
                return (int)this["bulkInsertSize"];
            }
            set { this["bulkInsertSize"] = value; }
        }

        [ConfigurationProperty("batchUpdateSize", IsRequired = false, DefaultValue = 100)]
        public int BatchUpdateSize {
            get {
                return (int)this["batchUpdateSize"];
            }
            set { this["batchUpdateSize"] = value; }
        }

        /// <summary>
        /// When pulling data from a source in key batches (to reduce blocking), 250 is the default, but you can mess around with this.  Keep it in multiples of the batchInsertSize though.
        /// </summary>
        [ConfigurationProperty("batchSelectSize", IsRequired = false, DefaultValue = 250)]
        public int BatchSelectSize {
            get {
                return (int)this["batchSelectSize"];
            }
            set { this["batchSelectSize"] = value; }
        }

    }
}