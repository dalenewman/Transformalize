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

        [ConfigurationProperty("compatabilityLevel", IsRequired = false, DefaultValue = 0)]
        public int CompatabilityLevel {
            get {
                return (int)this["compatabilityLevel"];
            }
            set { this["compatabilityLevel"] = value; }
        }

        [ConfigurationProperty("provider", IsRequired = false, DefaultValue = "")]
        public string Provider {
            get {
                return this["provider"] as string;
            }
            set { this["provider"] = value; }
        }

        [ConfigurationProperty("batchSize", IsRequired = false, DefaultValue = 500)]
        public int BatchSize {
            get {
                return (int)this["batchSize"];
            }
            set { this["batchSize"] = value; }
        }

    }
}