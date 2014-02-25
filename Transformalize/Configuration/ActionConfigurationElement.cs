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

namespace Transformalize.Configuration {
    public class ActionConfigurationElement : ConfigurationElement {

        private const string MODE = "mode";
        private const string FILE = "file";
        private const string ACTION = "action";
        private const string CONNECTION = "connection";
        private const string METHOD = "method";
        private const string URL = "url";
        private const string MODES = "modes";

        [ConfigurationProperty(ACTION, IsRequired = true)]
        public string Action {
            get { return this[ACTION] as string; }
            set { this[ACTION] = value; }
        }

        [ConfigurationProperty(FILE, IsRequired = false)]
        public string File {
            get { return this[FILE] as string; }
            set { this[FILE] = value; }
        }

        [ConfigurationProperty(MODE, IsRequired = false, DefaultValue = "*")]
        public string Mode {
            get { return this[MODE] as string; }
            set { this[MODE] = value; }
        }

        [ConfigurationProperty(CONNECTION, IsRequired = false)]
        public string Connection {
            get { return this[CONNECTION] as string; }
            set { this[CONNECTION] = value; }
        }

        [ConfigurationProperty(METHOD, IsRequired = false, DefaultValue = "get")]
        public string Method {
            get { return this[METHOD] as string; }
            set { this[METHOD] = value; }
        }

        [ConfigurationProperty(URL, IsRequired = false)]
        public string Url {
            get { return this[URL] as string; }
            set { this[URL] = value; }
        }

        [ConfigurationProperty(MODES)]
        public ModeElementCollection Modes {
            get { return this[MODES] as ModeElementCollection; }
        }

        public override bool IsReadOnly() {
            return false;
        }
    }
}