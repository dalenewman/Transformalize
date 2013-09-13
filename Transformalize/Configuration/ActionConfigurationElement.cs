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
    public class ActionConfigurationElement : ConfigurationElement
    {
        [ConfigurationProperty("action", IsRequired = true)]
        public string Action
        {
            get { return this["action"] as string; }
            set { this["action"] = value; }
        }

        [ConfigurationProperty("file", IsRequired = false)]
        public string File
        {
            get { return this["file"] as string; }
            set { this["file"] = value; }
        }

        [ConfigurationProperty("connection", IsRequired = false)]
        public string Connection
        {
            get { return this["connection"] as string; }
            set { this["connection"] = value; }
        }

        [ConfigurationProperty("method", IsRequired = false, DefaultValue = "get")]
        public string Method
        {
            get { return this["method"] as string; }
            set { this["method"] = value; }
        }

        [ConfigurationProperty("url", IsRequired = false)]
        public string Url
        {
            get { return this["url"] as string; }
            set { this["url"] = value; }
        }

        public override bool IsReadOnly()
        {
            return false;
        }
    }
}