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
    public class TemplateConfigurationElement : ConfigurationElement
    {
        private const string CONTENT_TYPE = "content-type";
        private const string FILE = "file";
        private const string SETTINGS = "settings";
        private const string ACTIONS = "actions";
        private const string NAME = "name";

        [ConfigurationProperty(NAME, IsRequired = true)]
        public string Name
        {
            get { return this[NAME] as string; }
            set { this[NAME] = value; }
        }

        [ConfigurationProperty(FILE, IsRequired = true)]
        public string File
        {
            get { return this[FILE] as string; }
            set { this[FILE] = value; }
        }

        [ConfigurationProperty(CONTENT_TYPE, IsRequired = false, DefaultValue = "raw")]
        public string ContentType
        {
            get { return this[CONTENT_TYPE] as string; }
            set { this[CONTENT_TYPE] = value; }
        }

        [ConfigurationProperty(SETTINGS)]
        public SettingElementCollection Settings
        {
            get { return this[SETTINGS] as SettingElementCollection; }
        }

        [ConfigurationProperty(ACTIONS)]
        public ActionElementCollection Actions
        {
            get { return this[ACTIONS] as ActionElementCollection; }
        }

        public override bool IsReadOnly()
        {
            return false;
        }
    }
}