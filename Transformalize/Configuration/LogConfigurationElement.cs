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
using Transformalize.Main;

namespace Transformalize.Configuration {

    public class LogConfigurationElement : ConfigurationElement {

        private const string NAME = "name";
        private const string PROVIDER = "provider";
        private const string LAYOUT = "layout";
        private const string LEVEL = "level";
        private const string CONNECTION = "connection";
        private const string FROM = "from";
        private const string TO = "to";
        private const string SUBJECT = "subject";
        private const string FILE = "file";
        private const string FOLDER = "folder";
        private const string ASYNC = "async";

        [ConfigurationProperty(NAME, IsRequired = true)]
        public string Name {
            get { return this[NAME] as string; }
            set { this[NAME] = value; }
        }

        [ConfigurationProperty(PROVIDER, IsRequired = false, DefaultValue = Common.DefaultValue)]
        public string Provider {
            get { return this[PROVIDER] as string; }
            set { this[PROVIDER] = value; }
        }

        [ConfigurationProperty(CONNECTION, IsRequired = false, DefaultValue = Common.DefaultValue)]
        public string Connection {
            get { return this[CONNECTION] as string; }
            set { this[CONNECTION] = value; }
        }

        [ConfigurationProperty(FILE, IsRequired = false, DefaultValue = Common.DefaultValue)]
        public string File {
            get { return this[FILE] as string; }
            set { this[File] = value; }
        }

        [ConfigurationProperty(FOLDER, IsRequired = false, DefaultValue = Common.DefaultValue)]
        public string Folder {
            get { return this[FOLDER] as string; }
            set { this[FOLDER] = value; }
        }

        [ConfigurationProperty(ASYNC, IsRequired = false, DefaultValue = false)]
        public bool Async {
            get { return (bool)this[ASYNC]; }
            set { this[ASYNC] = value; }
        }
        [ConfigurationProperty(SUBJECT, IsRequired = false, DefaultValue = Common.DefaultValue)]
        public string Subject {
            get { return this[SUBJECT] as string; }
            set { this[SUBJECT] = value; }
        }

        [ConfigurationProperty(FROM, IsRequired = false, DefaultValue = Common.DefaultValue)]
        public string From {
            get { return this[FROM] as string; }
            set { this[FROM] = value; }
        }

        [ConfigurationProperty(TO, IsRequired = false, DefaultValue = Common.DefaultValue)]
        public string To {
            get { return this[TO] as string; }
            set { this[TO] = value; }
        }

        [ConfigurationProperty(LEVEL, IsRequired = false, DefaultValue = "Info")]
        public string LogLevel {
            get { return this[LEVEL] as string; }
            set { this[LEVEL] = value.ToLower(); }
        }

        [ConfigurationProperty(LAYOUT, IsRequired = false, DefaultValue = Common.DefaultValue)]
        public string Layout {
            get { return this[LAYOUT] as string; }
            set { this[LAYOUT] = value; }
        }

        public override bool IsReadOnly() {
            return false;
        }

    }
}