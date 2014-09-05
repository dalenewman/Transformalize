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
    public class ParameterConfigurationElement : ConfigurationElement {
        private const string ENTITY = "entity";
        private const string FIELD = "field";
        private const string NAME = "name";
        private const string VALUE = "value";
        private const string INPUT = "input";
        private const string TYPE = "type";

        [ConfigurationProperty(ENTITY, IsRequired = false)]
        public string Entity {
            get { return this[ENTITY] as string; }
            set { this[ENTITY] = value; }
        }

        [ConfigurationProperty(FIELD, IsRequired = false)]
        public string Field {
            get { return this[FIELD] as string; }
            set { this[FIELD] = value; }
        }

        [ConfigurationProperty(NAME, IsRequired = false)]
        public string Name {
            get { return this[NAME] as string; }
            set { this[NAME] = value; }
        }

        [ConfigurationProperty(VALUE, IsRequired = false)]
        public string Value {
            get { return this[VALUE] as string; }
            set { this[VALUE] = value; }
        }

        [ConfigurationProperty(INPUT, IsRequired = false, DefaultValue = true)]
        public bool Input {
            get { return (bool)this[INPUT]; }
            set { this[INPUT] = value; }
        }

        [ConfigurationProperty(TYPE, IsRequired = false, DefaultValue = "System.String")]
        public string Type {
            get { return this[TYPE] as string; }
            set { this[TYPE] = value; }
        }

        public override bool IsReadOnly() {
            return false;
        }

        public bool HasValue() {
            return !string.IsNullOrEmpty(Value);
        }
    }
}