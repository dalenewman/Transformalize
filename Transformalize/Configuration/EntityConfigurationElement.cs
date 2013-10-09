#region License

// /*
// Transformalize - Replicate, Transform, and Denormalize Your Data...
// Copyright (C) 2013 Dale Newman
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// */

#endregion

using System.Configuration;

namespace Transformalize.Configuration
{
    public class EntityConfigurationElement : ConfigurationElement
    {
        private const string SCHEMA = "schema";
        private const string NAME = "name";
        private const string ALIAS = "alias";
        private const string CONNECTION = "connection";
        private const string FIELDS = "fields";
        private const string CALCULATED_FIELDS = "calculated-fields";
        private const string VERSION = "version";
        private const string OUTPUT = "output";
        private const string TRANSFORMS = "transforms";
        private const string AUTO = "auto";
        private const string PREFIX = "prefix";
        private const string GROUP = "group";
        private const string USE_BCP = "use-bcp";
        private const string INDEX_OPTIMIZATIONS = "index-optimizations";

        [ConfigurationProperty(SCHEMA, IsRequired = false, DefaultValue = "dbo")]
        public string Schema
        {
            get { return this[SCHEMA] as string; }
            set { this[SCHEMA] = value; }
        }

        [ConfigurationProperty(NAME, IsRequired = true)]
        public string Name
        {
            get { return this[NAME] as string; }
            set { this[NAME] = value; }
        }

        [ConfigurationProperty(ALIAS, IsRequired = false, DefaultValue = "")]
        public string Alias
        {
            get
            {
                var alias = this[ALIAS] as string;
                return alias == string.Empty ? Name : alias;
            }
            set { this[ALIAS] = value; }
        }

        [ConfigurationProperty(CONNECTION, IsRequired = false, DefaultValue = "input")]
        public string Connection
        {
            get { return this[CONNECTION] as string; }
            set { this[CONNECTION] = value; }
        }

        [ConfigurationProperty(FIELDS)]
        public FieldElementCollection Fields
        {
            get { return this[FIELDS] as FieldElementCollection; }
        }

        [ConfigurationProperty(CALCULATED_FIELDS)]
        public FieldElementCollection CalculatedFields
        {
            get { return this[CALCULATED_FIELDS] as FieldElementCollection; }
        }

        [ConfigurationProperty(VERSION, IsRequired = false, DefaultValue = "")]
        public string Version
        {
            get { return this[VERSION] as string; }
            set { this[VERSION] = value; }
        }

        [ConfigurationProperty(OUTPUT)]
        public OutputElementCollection Output
        {
            get { return this[OUTPUT] as OutputElementCollection; }
        }

        [ConfigurationProperty(TRANSFORMS)]
        public TransformElementCollection Transforms
        {
            get { return this[TRANSFORMS] as TransformElementCollection; }
        }

        [ConfigurationProperty(AUTO, IsRequired = false, DefaultValue = false)]
        public bool Auto
        {
            get { return (bool) this[AUTO]; }
            set { this[AUTO] = value; }
        }

        [ConfigurationProperty(INDEX_OPTIMIZATIONS, IsRequired = false, DefaultValue = true)]
        public bool IndexOptimizations {
            get { return (bool)this[INDEX_OPTIMIZATIONS]; }
            set { this[INDEX_OPTIMIZATIONS] = value; }
        }

        [ConfigurationProperty(USE_BCP, IsRequired = false, DefaultValue = false)]
        public bool UseBcp {
            get { return (bool)this[USE_BCP]; }
            set { this[USE_BCP] = value; }
        }

        [ConfigurationProperty(PREFIX, IsRequired = false, DefaultValue = "")]
        public string Prefix
        {
            get { return this[PREFIX] as string; }
            set { this[PREFIX] = value; }
        }

        [ConfigurationProperty(GROUP, IsRequired = false, DefaultValue = false)]
        public bool Group
        {
            get { return (bool) this[GROUP]; }
            set { this[GROUP] = value; }
        }

        public override bool IsReadOnly()
        {
            return false;
        }
    }
}