/*
Transformalize - Replicate, Transform, and Denormalize Your Data...
Copyright (C) 2013 Dale Newman

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System.Configuration;

namespace Transformalize.Configuration
{
    public class ConnectionConfigurationElement : ConfigurationElement {

        private const string NAME = "name";
        private const string VALUE = "value";
        private const string COMPATABILITY_LEVEL = "compatabilityLevel";
        private const string PROVIDER = "provider";
        private const string BATCH_SIZE = "batchSize";
        private const string ENABLED = "enabled";

        public override bool IsReadOnly()
        {
            return false;
        }

        [ConfigurationProperty(NAME, IsRequired = true)]
        public string Name {
            get {
                return this[NAME] as string;
            }
            set { this[NAME] = value; }
        }

        [ConfigurationProperty(VALUE, IsRequired = true)]
        public string Value {
            get {
                return this[VALUE] as string;
            }
            set { this[VALUE] = value; }
        }

        [ConfigurationProperty(COMPATABILITY_LEVEL, IsRequired = false, DefaultValue = 0)]
        public int CompatabilityLevel {
            get {
                return (int)this[COMPATABILITY_LEVEL];
            }
            set { this[COMPATABILITY_LEVEL] = value; }
        }

        [RegexStringValidator(@"(?i)SqlServer|AnalysisServices|MySql")]
        [ConfigurationProperty(PROVIDER, IsRequired = false, DefaultValue = "SqlServer")]
        public string Provider {
            get {
                return this[PROVIDER] as string;
            }
            set { this[PROVIDER] = value; }
        }

        [ConfigurationProperty(BATCH_SIZE, IsRequired = false, DefaultValue = 500)]
        public int BatchSize {
            get {
                return (int)this[BATCH_SIZE];
            }
            set { this[BATCH_SIZE] = value; }
        }

        [ConfigurationProperty(ENABLED, IsRequired = false, DefaultValue = true)]
        public bool Enabled
        {
            get
            {
                return (bool)this[ENABLED];
            }
            set { this[ENABLED] = value; }
        }

    }
}