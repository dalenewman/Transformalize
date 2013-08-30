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

namespace Transformalize.Configuration {

    public class ProcessConfigurationElement : ConfigurationElement {

        public override bool IsReadOnly()
        {
            return false;
        }

        [ConfigurationProperty("name", IsRequired = true)]
        public string Name {
            get {
                return this["name"] as string;
            }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("connections")]
        public ConnectionElementCollection Connections {
            get {
                return this["connections"] as ConnectionElementCollection;
            }
        }

        [ConfigurationProperty("maps")]
        public MapElementCollection Maps {
            get {
                return this["maps"] as MapElementCollection;
            }
        }

        [ConfigurationProperty("scripts")]
        public ScriptElementCollection Scripts
        {
            get
            {
                return this["scripts"] as ScriptElementCollection;
            }
        }

        [ConfigurationProperty("entities")]
        public EntityElementCollection Entities {
            get {
                return this["entities"] as EntityElementCollection;
            }
        }

        [ConfigurationProperty("relationships")]
        public RelationshipElementCollection Relationships {
            get {
                return this["relationships"] as RelationshipElementCollection;
            }
        }

        [ConfigurationProperty("calculated-fields")]
        public FieldElementCollection CalculatedFields
        {
            get
            {
                return this["calculated-fields"] as FieldElementCollection;
            }
        }

        [ConfigurationProperty("templates")]
        public TemplateElementCollection Templates
        {
            get
            {
                return this["templates"] as TemplateElementCollection;
            }
        }

    }
}