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
    public class OutputElementCollection : ConfigurationElementCollection
    {
        public EmptyConfigurationElement this[int index]
        {
            get { return BaseGet(index) as EmptyConfigurationElement; }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        [ConfigurationProperty("schema", IsRequired = false, DefaultValue = "dbo")]
        public string Schema
        {
            get { return this["schema"] as string; }
            set { this["schema"] = value; }
        }

        [ConfigurationProperty("name", IsRequired = false, DefaultValue = "output")]
        public string Name
        {
            get { return this["name"] as string; }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("connection", IsRequired = false, DefaultValue = "output")]
        public string Connection
        {
            get { return this["connection"] as string; }
            set { this["connection"] = value; }
        }

        public override bool IsReadOnly()
        {
            return false;
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new EmptyConfigurationElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return element.GetHashCode();
        }
    }
}