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

namespace Transformalize.Configuration {
    public class EntityConfigurationElement : ConfigurationElement {

        [ConfigurationProperty("schema", IsRequired = false, DefaultValue = "dbo")]
        public string Schema {
            get {
                return this["schema"] as string;
            }
            set { this["schema"] = value; }
        }

        [ConfigurationProperty("name", IsRequired = true)]
        public string Name {
            get {
                return this["name"] as string;
            }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("connection", IsRequired = false, DefaultValue = "input")]
        public string Connection {
            get {
                return this["connection"] as string;
            }
            set { this["connection"] = value; }
        }

        [ConfigurationProperty("primaryKey")]
        public FieldElementCollection PrimaryKey {
            get {
                return this["primaryKey"] as FieldElementCollection;
            }
        }

        [ConfigurationProperty("fields")]
        public FieldElementCollection Fields {
            get {
                return this["fields"] as FieldElementCollection;
            }
        }

        [ConfigurationProperty("version", IsRequired = true)]
        public string Version {
            get {
                return this["version"] as string;
            }
            set { this["version"] = value; }
        }

        [ConfigurationProperty("output")]
        public OutputElementCollection Output {
            get {
                return this["output"] as OutputElementCollection;
            }
        }

        [ConfigurationProperty("transforms")]
        public TransformElementCollection Transforms {
            get {
                return this["transforms"] as TransformElementCollection;
            }
        }

        [ConfigurationProperty("auto", IsRequired = false, DefaultValue = false)]
        public bool Auto {
            get {
                return (bool) this["auto"];
            }
            set { this["auto"] = value; }
        }
    }
}