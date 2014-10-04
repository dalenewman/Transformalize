using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Transformalize.Libs.Sqloogle.Processes;
using Transformalize.Main;
using Transformalize.Main.Providers;

namespace Transformalize.Libs.Sqloogle
{
    public class SqloogleBotConfiguration : ConfigurationSection
    {
        [ConfigurationProperty("searchIndexPath", DefaultValue = "c:\\sqloogle\\searchIndex\\", IsRequired = true)]
        public string SearchIndexPath
        {
            get
            {
                return this["searchIndexPath"] as string;
            }
        }

        [ConfigurationProperty("filePath", DefaultValue = "", IsRequired = false)]
        public string FilePath
        {
            get
            {
                return this["filePath"] as string;
            }
        }

        [ConfigurationProperty("servers")]
        public ServerElementCollection Servers
        {
            get
            {
                return this["servers"] as ServerElementCollection;
            }
        }

        [ConfigurationProperty("skips")]
        public SkipElementCollection Skips
        {
            get
            {
                return this["skips"] as SkipElementCollection;
            }
        }

        public IEnumerable<ServerCrawlProcess> ServerCrawlProcesses(Process process, AbstractConnection connection)
        {
            return
                (from ServerConfigurationElement server in Servers
                 select new ServerCrawlProcess(process, connection));
        } 
   
    }

    public class ServerConfigurationElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get
            {
                return this["name"] as string;
            }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("connectionString", IsRequired = true)]
        public string ConnectionString
        {
            get
            {
                return this["connectionString"] as string;
            }
            set { this["connectionString"] = value; }
        }

    }

    public class ServerElementCollection : ConfigurationElementCollection
    {
        public ServerConfigurationElement this[int index]
        {
            get
            {
                return base.BaseGet(index) as ServerConfigurationElement;
            }
            set
            {
                if (base.BaseGet(index) != null)
                {
                    base.BaseRemoveAt(index);
                }
                this.BaseAdd(index, value);
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new ServerConfigurationElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ServerConfigurationElement)element).Name;
        }
    }

    public class SkipConfigurationElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get { return ((string)this["name"]).ToLower(); }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("operator", IsRequired = false, DefaultValue = "Equals")]
        public string Operator
        {
            get { return ((string) this["operator"]).ToLower(); }
            set { this["operator"] = value; }
        }

    }


    public class SkipElementCollection : ConfigurationElementCollection
    {
        public SkipConfigurationElement this[int index]
        {
            get
            {
                return base.BaseGet(index) as SkipConfigurationElement;
            }
            set
            {
                if (base.BaseGet(index) != null)
                {
                    base.BaseRemoveAt(index);
                }
                this.BaseAdd(index, value);
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new SkipConfigurationElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((SkipConfigurationElement)element).Name;
        }

        public IEnumerable<SkipConfigurationElement> ToEnumerable()
        {
            var skips = new List<SkipConfigurationElement>();
            for (int i = 0; i < this.Count; i++)
                skips.Add(this.BaseGet(i) as SkipConfigurationElement);
            return skips;
        }
 
        public IEnumerable<string> ToNames(string op)
        {
            return this.Count == 0 ? new List<string>() : this.ToEnumerable().Where(s=>s.Operator == op).Select(s => s.Name);
        }

        public string ToInExpression()
        {
            if (this.Count == 0)
                return "''";

            var databases = from string name in this.ToNames("equals") select "'" + name + "'";
            return string.Join(",", databases.ToArray());
        }

        public IEnumerable<string> ToLikeExpressions()
        {
            var databases = new List<string>();
            databases.AddRange(from string name in this.ToNames("startswith") select "'" + name + "%'");
            databases.AddRange(from string name in this.ToNames("endswith") select "'%" + name + "'");
            databases.AddRange(from string name in this.ToNames("contains") select "'%" + name + "%'");
            return databases;
        }

        public bool Match(string database)
        {
            database = database.ToLower();
            if (this.ToNames("equals").Any(database.Equals)) {
                return true;
            }

            if (this.ToNames("startswith").Any(database.StartsWith)) {
                return true;
            }

            return this.ToNames("endswith").Any(database.EndsWith) || this.ToNames("contains").Any(database.Contains);
        }
    }

}
