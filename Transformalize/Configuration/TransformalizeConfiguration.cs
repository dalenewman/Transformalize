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
using System.IO;
using System.Text;
using System.Xml;

namespace Transformalize.Configuration
{
    public class TransformalizeConfiguration : ConfigurationSection
    {
        [ConfigurationProperty("processes")]
        public ProcessElementCollection Processes
        {
            get { return this["processes"] as ProcessElementCollection; }
        }

        public override bool IsReadOnly()
        {
            return false;
        }

        public string Serialize(ConfigurationElement parentElement, string name, ConfigurationSaveMode saveMode)
        {
            return SerializeSection(parentElement, name, saveMode);
        }

        public void Deserialize(string serializedConfiguration)
        {
            var reader = XmlReader.Create(new StringReader(serializedConfiguration));
            if (!reader.ReadToFollowing("transformalize")) return;
            var stringBuilder = new StringBuilder().Append(reader.ReadOuterXml());
            var stringReader = new StringReader(stringBuilder.ToString());
            reader = XmlReader.Create(stringReader);
            DeserializeSection(reader);
        }
    }
}