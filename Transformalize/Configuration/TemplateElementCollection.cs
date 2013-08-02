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

using System;
using System.Configuration;

namespace Transformalize.Configuration {
    public class TemplateElementCollection : ConfigurationElementCollection {

        public TemplateConfigurationElement this[int index] {
            get {
                return BaseGet(index) as TemplateConfigurationElement;
            }
            set {
                if (BaseGet(index) != null) {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        protected override ConfigurationElement CreateNewElement() {
            return new TemplateConfigurationElement();
        }

        protected override object GetElementKey(ConfigurationElement element) {
            return ((TemplateConfigurationElement)element).Name.ToLower();
        }

        [ConfigurationProperty("path", IsRequired = false, DefaultValue = "")]
        public string Path
        {
            get
            {
                return this["path"] as string;
            }
            set { this["path"] = value; }
        }

    }
}