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
    public class XmlConfigurationElement : ConfigurationElement {

        [ConfigurationProperty("schema", IsRequired = false, DefaultValue = "")]
        public string Schema {
            get {
                return this["schema"] as string;
            }
            set { this["schema"] = value; }
        }

        [ConfigurationProperty("xPath", IsRequired = true)]
        public string XPath {
            get {
                return this["xPath"] as string;
            }
            set { this["xPath"] = value; }
        }

        [ConfigurationProperty("alias", IsRequired = false, DefaultValue = "")]
        public string Alias {
            get
            {
                var alias = this["alias"] as string;
                return alias == null || alias.Equals(string.Empty) ? XPath : alias;
            }
            set { this["alias"] = value; }
        }

        [ConfigurationProperty("index", IsRequired = false, DefaultValue = 1)]
        public int Index {
            get {
                return (int)this["index"];
            }
            set { this["index"] = value; }
        }

        [ConfigurationProperty("type", IsRequired = false, DefaultValue = "System.String")]
        public string Type {
            get {
                return this["type"] as string;
            }
            set { this["type"] = value; }
        }

        [ConfigurationProperty("length", IsRequired = false, DefaultValue=64)]
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

        [ConfigurationProperty("output", IsRequired = false, DefaultValue = true)]
        public bool Output {
            get {
                return (bool)this["output"];
            }
            set { this["output"] = value; }
        }

        [ConfigurationProperty("default", IsRequired = false, DefaultValue = null)]
        public string Default {
            get {
                return (string)this["default"];
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