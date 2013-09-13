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
    public class ParameterConfigurationElement : ConfigurationElement
    {
        [ConfigurationProperty("entity", IsRequired = false)]
        public string Entity
        {
            get { return this["entity"] as string; }
            set { this["entity"] = value; }
        }

        [ConfigurationProperty("field", IsRequired = false)]
        public string Field
        {
            get { return this["field"] as string; }
            set { this["field"] = value; }
        }

        [ConfigurationProperty("name", IsRequired = false)]
        public string Name
        {
            get { return this["name"] as string; }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("value", IsRequired = false)]
        public string Value
        {
            get { return this["value"] as string; }
            set { this["value"] = value; }
        }

        [ConfigurationProperty("type", IsRequired = false, DefaultValue = "System.String")]
        public string Type
        {
            get { return this["type"] as string; }
            set { this["type"] = value; }
        }

        public override bool IsReadOnly()
        {
            return false;
        }

        public bool HasValue()
        {
            return !string.IsNullOrEmpty(Value);
        }
    }
}