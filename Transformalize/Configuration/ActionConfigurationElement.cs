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

using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Transformalize.Configuration {
    public class ActionConfigurationElement : ConfigurationElement {

        private const string MODE = "mode";
        private const string FILE = "file";
        private const string ACTION = "action";
        private const string CONNECTION = "connection";
        private const string METHOD = "method";
        private const string URL = "url";
        private const string MODES = "modes";
        private const string FROM = "from";
        private const string TO = "to";
        private const string CC = "cc";
        private const string BCC = "bcc";
        private const string SUBJECT = "subject";
        private const string ARGUMENTS = "arguments";
        private const string HTML = "html";
        private const string BEFORE = "before";
        private const string AFTER = "after";
        private const string CONDITIONAL = "conditional";
        private const string OLD_VALUE = "old-value";
        private const string NEW_VALUE = "new-value";
        private const string BODY = "body";
        private const string COMMAND = "command";
        private const string TIME_OUT = "time-out";

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

        [ConfigurationProperty(COMMAND, IsRequired = false, DefaultValue = "")]
        public string Command {
            get { return this[COMMAND] as string; }
            set { this[COMMAND] = value; }
        }

        [ConfigurationProperty(TIME_OUT, IsRequired = false, DefaultValue = 0)]
        public int TimeOut {
            get { return (int) this[TIME_OUT]; }
            set { this[TIME_OUT] = value; }
        }

        [ConfigurationProperty(MODE, IsRequired = false, DefaultValue = "")]
        public string Mode {
            get { return this[MODE] as string; }
            set { this[MODE] = value; }
        }

        [ConfigurationProperty(CONNECTION, IsRequired = false, DefaultValue = "")]
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

        [ConfigurationProperty(FROM, IsRequired = false)]
        public string From {
            get { return this[FROM] as string; }
            set { this[FROM] = value; }
        }

        [ConfigurationProperty(TO, IsRequired = false)]
        public string To {
            get { return this[TO] as string; }
            set { this[TO] = value; }
        }

        [ConfigurationProperty(CC, IsRequired = false)]
        public string Cc {
            get { return this[CC] as string; }
            set { this[CC] = value; }
        }

        [ConfigurationProperty(BCC, IsRequired = false)]
        public string Bcc {
            get { return this[BCC] as string; }
            set { this[BCC] = value; }
        }

        [ConfigurationProperty(ARGUMENTS, IsRequired = false)]
        public string Arguments {
            get { return this[ARGUMENTS] as string; }
            set { this[ARGUMENTS] = value; }
        }

        [ConfigurationProperty(BEFORE, IsRequired = false, DefaultValue = false)]
        public bool Before {
            get { return (bool)this[BEFORE]; }
            set { this[BEFORE] = value; }
        }

        [ConfigurationProperty(AFTER, IsRequired = false, DefaultValue = true)]
        public bool After {
            get { return (bool)this[AFTER]; }
            set { this[AFTER] = value; }
        }

        [ConfigurationProperty(HTML, IsRequired = false, DefaultValue = true)]
        public bool Html {
            get { return (bool) this[HTML]; }
            set { this[HTML] = value; }
        }

        [ConfigurationProperty(BODY, IsRequired = false, DefaultValue = "")]
        public string Body {
            get { return this[BODY] as string; }
            set { this[BODY] = value; }
        }

        [ConfigurationProperty(OLD_VALUE, IsRequired = false, DefaultValue = "")]
        public string OldValue {
            get { return this[OLD_VALUE] as string; }
            set { this[OLD_VALUE] = value; }
        }

        [ConfigurationProperty(NEW_VALUE, IsRequired = false, DefaultValue = "")]
        public string NewValue {
            get { return this[NEW_VALUE] as string; }
            set { this[NEW_VALUE] = value; }
        }

        [ConfigurationProperty(SUBJECT, IsRequired = false, DefaultValue = "")]
        public string Subject {
            get { return this[SUBJECT] as string; }
            set { this[SUBJECT] = value; }
        }

        [ConfigurationProperty(MODES)]
        public ModeElementCollection Modes {
            get { return this[MODES] as ModeElementCollection; }
        }

        [ConfigurationProperty(CONDITIONAL, IsRequired = false, DefaultValue = false)]
        public bool Conditional {
            get { return (bool)this[CONDITIONAL]; }
            set { this[CONDITIONAL] = value; }
        }

        public override bool IsReadOnly() {
            return false;
        }

        public string[] GetModes() {
            var modes = new List<string>();

            if (Mode != string.Empty) {
                modes.Add(Mode);
            }

            modes.AddRange(from ModeConfigurationElement mode in Modes select mode.Mode);
            return modes.ToArray();
        }

    }
}