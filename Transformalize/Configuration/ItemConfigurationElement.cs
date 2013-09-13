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
    public class ItemConfigurationElement : ConfigurationElement
    {
        [ConfigurationProperty("from", IsRequired = true)]
        public string From
        {
            get { return this["from"] as string; }
            set { this["from"] = value; }
        }

        [ConfigurationProperty("to", IsRequired = false, DefaultValue = "")]
        public string To
        {
            get { return this["to"] as string; }
            set { this["to"] = value; }
        }

        [ConfigurationProperty("parameter", IsRequired = false, DefaultValue = "")]
        public string Parameter
        {
            get { return this["parameter"] as string; }
            set { this["parameter"] = value; }
        }

        [ConfigurationProperty("operator", IsRequired = false, DefaultValue = "equals")]
        public string Operator
        {
            get { return this["operator"] as string; }
            set { this["operator"] = value; }
        }

        public override bool IsReadOnly()
        {
            return false;
        }
    }
}